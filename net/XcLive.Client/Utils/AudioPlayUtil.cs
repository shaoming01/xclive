using System.Collections.Concurrent;
using Frame.Ext;
using NAudio.Wave;

namespace Frame.Utils;

public class AudioPlayUtil
{
    private readonly ConcurrentQueue<VoicePlayData> _queue = new();
    private readonly ConcurrentQueue<VoicePlayData> _insertQueue = new();
    private VoicePlayData? _currentMainVoice;
    private VoicePlayData? _currentInsertVoice;

    private readonly WaveOutEvent _waveOutEvent = new();

    private WaveFileReader? _mainAudioReader { get; set; }
    private WaveFileReader? _insertedAudioReader { get; set; }
    private bool _isOriginalAudioPaused; //原视频暂停，代表正在播放插入任务
    private bool _needFirstStart = true; //初始时或所有任务都完成了，再次播放需要冷启动一次

    public WaveFileReader? MainAudioReader => _mainAudioReader;
    public WaveFileReader? InsertedAudioReader => _insertedAudioReader;
    public bool IsOriginalAudioPaused => _isOriginalAudioPaused;
    public WaveOutEvent WaveOutEvent => _waveOutEvent;
    public ConcurrentQueue<VoicePlayData> Queue => _queue;
    public ConcurrentQueue<VoicePlayData> InsertQueue1 => _insertQueue;

    public event EventHandler<VoicePlayMessage>? OnMessage;

    public AudioPlayUtil()
    {
        _waveOutEvent.PlaybackStopped += PlayStopped;
        IniInsertTask();
    }

    private string _currentSoundCardId = "-1";

    public void PlayVoice(VoicePlayData data)
    {
        try
        {
            if (IsDuplicate(data))
            {
                return;
            }

            SetSoundCard(data.soundCardId);
            if (data.playType == VoicePlayType.Append)
            {
                _queue.Enqueue(data);
            }
            else if (data.playType == VoicePlayType.Insert)
            {
                _insertQueue.Enqueue(data);
            }
            else
            {
                Stop();
                _queue.Enqueue(data);
            }

            StartFirstPlay();
        }
        catch (Exception e)
        {
            _needFirstStart = true;
            Log4.Log.Error(e);
        }
    }

    private bool IsDuplicate(VoicePlayData data)
    {
        if (_queue.Any(q => q.id == data.id))
        {
            return true;
        }
        else if (_insertQueue.Any(q => q.id == data.id))
        {
            return true;
        }
        else if (_currentMainVoice?.id == data.id)
        {
            return true;
        }
        else if (_currentInsertVoice?.id == data.id)
        {
            return true;
        }

        return false;
    }


    private void PlayStopped(object? sender, StoppedEventArgs e)
    {
        try
        {
//普通任务播放完了，发送个消息
            if (!_isOriginalAudioPaused && _currentMainVoice != null)
            {
                OnMessage?.Invoke(this, new VoicePlayMessage
                {
                    id = _currentMainVoice.id,
                    playType = _currentMainVoice.playType,
                    playStatus = 1,
                    TaskCount = _queue.Count,
                    InsertTaskCount = _insertQueue.Count,
                });
            }

            if (_insertQueue.TryDequeue(out var voice)) //有插入视频优先播放，即使原先是插入状态，还是继续插入，反正插入的总是优先
            {
                _currentInsertVoice = voice;
                var memoryStream = new MemoryStream(voice.Data);
                _insertedAudioReader = new WaveFileReader(memoryStream);
                _waveOutEvent.Init(_insertedAudioReader); // 设置为插入的音频
                _waveOutEvent.Volume = voice.volume;
                _waveOutEvent.Play(); // 播放插入的音频
            }
            else if (_isOriginalAudioPaused && _mainAudioReader != null) //所有插入的播放完了，恢复正常播放
            {
                _isOriginalAudioPaused = false;
                _waveOutEvent.Init(_mainAudioReader);
                if (_currentMainVoice != null)
                {
                    _waveOutEvent.Volume = _currentMainVoice.volume;
                }

                _waveOutEvent.Play();
            }
            else if (_queue.TryDequeue(out var voiceNormal))
            {
                _currentMainVoice = voiceNormal;
                var memoryStream = new MemoryStream(voiceNormal.Data);
                _mainAudioReader = new WaveFileReader(memoryStream);
                _waveOutEvent.Init(_mainAudioReader);
                _waveOutEvent.Volume = voiceNormal.volume;
                _waveOutEvent.Play();
            }
            else
            {
                //所有内容都没了
                _needFirstStart = true;
            }
        }
        catch (Exception exception)
        {
            Log4.Log.Error(exception);
        }
    }

    private void IniInsertTask()
    {
        Task.Factory.StartNew(() =>
        {
            while (true) //持续循环运行，等待下一个插入任务
            {
                //主视频暂停了，说明在播放插入的，插入播放完成会自动取下一条插入视频的，这条线程不用处理
                while (_isOriginalAudioPaused || _insertQueue.Count == 0)
                {
                    Thread.Sleep(100);
                }

                var pos = GetCurrentPosition();
                //小于5秒，不插入了
                if (_currentMainVoice == null || pos.TotalTime - pos.Position < new TimeSpan(0, 0, 5))
                {
                    Thread.Sleep(100);
                    continue;
                }

                var silentList = AudioSilenceDetector.FindTopSilencePoints(_currentMainVoice.Data);
                var point = silentList.FirstOrDefault(s => s - pos.Position > new TimeSpan(0, 0, 2));
                //未找到合适断点，不强行中断
                if (point == TimeSpan.Zero)
                {
                    continue;
                }

                StartInsert(point);
            }
        });
    }

    private void StartInsert(TimeSpan point)
    {
        //等待它到断点
        while (true)
        {
            var pos = GetCurrentPosition();
            if (pos.Position < point)
            {
                Thread.Sleep(100);
            }
            else
            {
                break;
            }
        }

        try
        {
            _isOriginalAudioPaused = true; // 标记原音频已暂停
            _waveOutEvent.Stop(); // 停止播放原音频
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
            _isOriginalAudioPaused = false;
            _waveOutEvent.Init(_mainAudioReader);
            _waveOutEvent.Play();
        }
    }

    private static void InsertQueue(ConcurrentQueue<VoicePlayData> queue, VoicePlayData result)
    {
        var list = queue.ToList();
        queue.Clear();
        queue.Enqueue(result);
        list.ForEach(queue.Enqueue);
    }


    private void StartFirstPlay()
    {
        if (_waveOutEvent.PlaybackState == PlaybackState.Playing)
        {
            //正在播放不用管了，反正已经加入到队列里去了，播放完后会继续播放的
            return;
        }
        else if (_waveOutEvent.PlaybackState == PlaybackState.Paused)
        {
            //手工操作的暂停或为了插入而临时暂停，都不用处理
            return;
        }
        else if (_needFirstStart)
        {
            //主任务冷启动
            if (_queue.TryDequeue(out var voice))
            {
                _needFirstStart = false;
                var memoryStream = new MemoryStream(voice.Data);
                _currentMainVoice = voice;
                // 创建音频读取器（假设音频是 WAV 格式）
                _mainAudioReader = new WaveFileReader(memoryStream);
                _waveOutEvent.Init(_mainAudioReader);
                _waveOutEvent.Volume = _currentMainVoice.volume;
                _waveOutEvent.Play();
            }
            else if (_insertQueue.TryDequeue(out var insertVoice)) //插入任务冷启动
            {
                _needFirstStart = false;
                _isOriginalAudioPaused = true;
                var memoryStream = new MemoryStream(insertVoice.Data);
                _currentInsertVoice = insertVoice;
                // 创建音频读取器（假设音频是 WAV 格式）
                _insertedAudioReader = new WaveFileReader(memoryStream);
                _waveOutEvent.Init(_insertedAudioReader);
                _waveOutEvent.Volume = _currentInsertVoice.volume;
                _waveOutEvent.Play();
            }
        }
    }


    private PlayState GetCurrentPosition()
    {
        if (_mainAudioReader == null)
        {
            return new PlayState
            {
                Position = TimeSpan.Zero,
                TotalTime = TimeSpan.Zero
            };
        }

        TimeSpan currentTime = _mainAudioReader.CurrentTime;
        return new PlayState
        {
            Position = currentTime,
            TotalTime = _mainAudioReader.TotalTime
        };
    }

    public class PlayState
    {
        public TimeSpan Position { get; set; }
        public TimeSpan TotalTime { get; set; }
    }

    // 停止音频播放
    public void Stop()
    {
        try
        {
            _queue.Clear();
            _insertQueue.Clear();
            _waveOutEvent.Stop();
            _isOriginalAudioPaused = false;
            _currentMainVoice = null;
            _currentInsertVoice = null;
            _mainAudioReader = null;
            _insertedAudioReader = null;
            _needFirstStart = true;
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
        }
    }

    public void Pause()
    {
        try
        {
            _waveOutEvent.Pause();
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
        }
    }

    public void ResumePlay()
    {
        try
        {
            if (_waveOutEvent.PlaybackState == PlaybackState.Paused)
            {
                _waveOutEvent.Play();
            }
        }
        catch (Exception e)
        {
            Log4.Log.Error(e);
        }
    }

    private void SetSoundCard(string dataSoundCardId)
    {
        if (dataSoundCardId == _currentSoundCardId)
        {
            return;
        }

        var list = new SoundCardManager().GetAllOutputDevices();
        var card = list.FirstOrDefault(l => l.Id == dataSoundCardId);
        if (card == null)
        {
            return;
        }

        for (int i = 0; i < WaveOut.DeviceCount; i++)
        {
            var device = WaveOut.GetCapabilities(i);
            if (card.Name.StartsWith(device.ProductName))
            {
                _waveOutEvent.DeviceNumber = i;
                _currentSoundCardId = dataSoundCardId;
                break;
            }
        }
    }

    public VoicePlayStatus GetStatus()
    {
        return new VoicePlayStatus()
        {
            playState = (int)_waveOutEvent.PlaybackState,
            isInsert = _isOriginalAudioPaused,
            mainAudioTotal = _mainAudioReader == null ? 0 : (int)_mainAudioReader.TotalTime.TotalMilliseconds,
            mainAudioCurrent = _mainAudioReader == null ? 0 : (int)_mainAudioReader.CurrentTime.TotalMilliseconds,
            insertAudioTotal = _insertedAudioReader == null ? 0 : (int)_insertedAudioReader.TotalTime.TotalMilliseconds,
            insertAudioCurrent = _insertedAudioReader == null
                ? 0
                : (int)_insertedAudioReader.CurrentTime.TotalMilliseconds,
            mainQueueCount = _queue.Count,
            insertQueueCount = _insertQueue.Count,
            mainId = _currentMainVoice?.id ?? "",
            insertId = _currentInsertVoice?.id ?? "",
            mainText = _currentMainVoice?.text ?? "",
            insertText = _currentInsertVoice?.text ?? "",
        };
    }

    public bool isIdInQueue(string id)
    {
        if (_queue.Any(q => q.id == id))
        {
            return true;
        }

        if (_insertQueue.Any(q => q.id == id))
        {
            return true;
        }

        if (_currentInsertVoice?.id == id)
        {
            return true;
        }

        if (_currentInsertVoice?.id == id)
        {
            return true;
        }

        return false;
    }
}

[Flags]
public enum VoicePlayType
{
    /// <summary>
    /// 普通追加到播放列表
    /// </summary>
    Append = 1,

    /// <summary>
    /// 到安全位置后，插入本语音
    /// </summary>
    Insert = 2,

    /// <summary>
    /// 到安全位置后清除任务，再播放本语音
    /// </summary>
    ClearToPlay = 3,
}

public class VoicePlayData
{
    public string? id { get; set; }
    public string soundCardId { get; set; }
    public VoicePlayType playType { get; set; }
    public string voice { get; set; }
    public string text { get; set; }
    public float volume { get; set; }

    public byte[] Data
    {
        get
        {
            if (!voice.Has())
            {
                return [];
            }

            return Convert.FromBase64String(voice);
        }
    }
}

public class VoicePlayMessage
{
    public string? id { get; set; }
    public VoicePlayType playType { get; set; }

    /// <summary>
    /// 1,完成；
    /// </summary>
    public int playStatus { get; set; }

    public int TaskCount { get; set; }
    public int InsertTaskCount { get; set; }
}

public class VoicePlayStatus
{
    /// <summary>
    /// 0,stoped,1,Playing,2,Paused
    /// </summary>
    public int playState { get; set; }

    public bool isInsert { get; set; }
    public int mainAudioTotal { get; set; }
    public int mainAudioCurrent { get; set; }
    public int insertAudioTotal { get; set; }
    public int insertAudioCurrent { get; set; }
    public int mainQueueCount { get; set; }
    public int insertQueueCount { get; set; }
    public string mainId { get; set; }
    public string insertId { get; set; }
    public string mainText { get; set; }
    public string insertText { get; set; }
}