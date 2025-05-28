using Frame.Ext;
using Frame.Utils;

namespace Frame;

public partial class AudioTestForm : Form
{
    AudioPlayUtil _audioPlayUtil = new();

    public AudioTestForm()
    {
        InitializeComponent();
        IniStatusSyncTask();
        IniVolume();
    }

    private void IniVolume()
    {
        var list = new SoundCardManager().GetAllOutputDevices();
        combSoundCard.Items.Clear();
        combSoundCard.SelectedValueChanged += SoundCardChanged;
        list.ForEach(l => { combSoundCard.Items.Add(l); });
        
    }

    private void SoundCardChanged(object? sender, EventArgs e)
    {
        var item = combSoundCard.SelectedItem as SoundCardInfo;
        if (item == null)
        {
            return;
        }

        var val = new SoundCardManager().GetVolume(item.Id);
        trackBar1.Value = val;
    }

    private void IniStatusSyncTask()
    {
        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                var play = _audioPlayUtil;
                try
                {
                    var t1 =
                        $"主播放进度：{play.MainAudioReader?.CurrentTime:mm\\:ss\\.fff}/{play.MainAudioReader?.TotalTime:mm\\:ss\\.fff}";
                    var t2 =
                        $"插队播放进度：{play.InsertedAudioReader?.CurrentTime:mm\\:ss\\.fff}/{play.InsertedAudioReader?.TotalTime:mm\\:ss\\.fff}";
                    var t3 = $"主任务被插队暂停：{play.IsOriginalAudioPaused}";
                    var t4 = $"播放器状态：{play.WaveOutEvent.PlaybackState}";
                    var t5 = $"播放队列数/插入队列数：{play.Queue.Count}/{play.InsertQueue1.Count}";
                    Invoke(() => { lblPlayStatus.Text = t1 + "\n" + t2 + "\n" + t3 + "\n" + t4 + "\n" + t5; });
                }
                catch (Exception e)
                {
                }


                Thread.Sleep(200);
            }
        });
    }

    private void btnNormalTask_Click(object sender, EventArgs e)
    {
        var text = txtNormal.Text;
        var lines = text.SplitEx('\n', true);
        lines.ForEach(l =>
        {
            if (!File.Exists(l))
            {
                return;
            }

            var by = File.ReadAllBytes(l);
            var data = new VoicePlayData()
            {
                id = "",
                soundCardId = "-1",
                playType = VoicePlayType.Append,
                voice = Convert.ToBase64String(by)
            };
            _audioPlayUtil.PlayVoice(data);
        });
    }

    private void btnClearToPlay_Click(object sender, EventArgs e)
    {
        var text = txtNormal.Text;
        var lines = text.SplitEx('\n', true);
        lines.ForEach(l =>
        {
            if (!File.Exists(l))
            {
                return;
            }

            var by = File.ReadAllBytes(l);
            var data = new VoicePlayData()
            {
                id = "",
                soundCardId = "-1",
                playType = VoicePlayType.ClearToPlay,
                voice = Convert.ToBase64String(by)
            };

            _audioPlayUtil.PlayVoice(data);
        });
    }

    private void btnInsert_Click(object sender, EventArgs e)
    {
        var text = txtInsert.Text;
        var lines = text.SplitEx('\n', true);
        lines.ForEach(l =>
        {
            if (!File.Exists(l))
            {
                return;
            }

            var by = File.ReadAllBytes(l);
            var data = new VoicePlayData()
            {
                id = "",
                soundCardId = "-1",
                playType = VoicePlayType.Insert,
                voice = Convert.ToBase64String(by)
            };
            _audioPlayUtil.PlayVoice(data);
        });
    }


    private void btnPlay_Click(object sender, EventArgs e)
    {
        _audioPlayUtil.ResumePlay();
    }

    private void btnPause_Click(object sender, EventArgs e)
    {
        _audioPlayUtil.Pause();
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        _audioPlayUtil.Stop();
    }

    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        var soundCard = combSoundCard.SelectedItem as SoundCardInfo;
        if (soundCard == null)
        {
            return;
        }

        var ctl = new SoundCardManager();
        ctl.SetVolume(soundCard.Id, trackBar1.Value);
    }
}