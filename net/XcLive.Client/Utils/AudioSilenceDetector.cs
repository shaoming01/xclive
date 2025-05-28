using NAudio.Wave;

namespace Frame.Utils;

public class AudioSilenceDetector
{
    private const int SampleRate = 32000; // 假设音频采样率为44100Hz，适用于WAV文件
    private const int SegmentDurationMillis = 100; // 每段持续时间（毫秒）
    private const int TopCount = 5; // 我们需要找出最可能的5个停顿点

    public static List<TimeSpan> FindTopSilencePoints(byte[] audioBytes)
    {
        var silencePoints = new List<TimeSpan>();
        TimeSpan totalTime = TimeSpan.Zero;

        // 使用 MemoryStream 和 WaveFileReader 来读取字节数组
        using (var memoryStream = new MemoryStream(audioBytes))
        using (var reader = new WaveFileReader(memoryStream))
        {
            totalTime = reader.TotalTime;
            var buffer = new byte[reader.Length]; // 缓冲区
            var waveBuffer = new WaveBuffer(buffer); // 将字节数据转换为 WaveBuffer
            int samplesRead;
            int totalSamples = 0;

            List<float> audioAmplitudes = new List<float>();

            // 读取所有音频数据，计算每个样本的振幅
            while ((samplesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
            {
                // 每次读取的数据都会填充到WaveBuffer，转换为16位PCM数据
                for (int i = 0; i < samplesRead / 2; i++) // 每个样本2字节（16位）
                {
                    short sample = waveBuffer.ShortBuffer[i]; // 获取音频样本（16位）
                    audioAmplitudes.Add(Math.Abs(sample) / 32768f); // 将16位PCM数据转换为浮动值（0到1之间）
                    totalSamples++;
                }
            }

            // 计算每段音频的样本数（100毫秒）
            int segmentSize = (int)((SampleRate * SegmentDurationMillis) / 1000); // 100ms内有多少个样本

            List<(int start, float avgAmplitude)> segmentAmplitudes = new List<(int, float)>();

            // 将音频数据分割成段，计算每段的平均振幅
            for (int i = 0; i < audioAmplitudes.Count; i += segmentSize)
            {
                // 确定当前段的样本数
                var segment = audioAmplitudes.Skip(i).Take(segmentSize).ToList();

                // 如果当前段不足200毫秒的样本数，跳过
                if (segment.Count == 0) break;

                // 计算当前段的振幅平均值
                float avgAmplitude = segment.Average();

                // 将结果保存
                segmentAmplitudes.Add((i, avgAmplitude));
            }

            // 对振幅平均值进行排序，按从小到大排序
            var topSilences = segmentAmplitudes
                .OrderBy(x => x.avgAmplitude)
                .ToList();

            // 计算对应的时间戳
            foreach (var silence in topSilences)
            {
                var silenceTime = TimeSpan.FromSeconds(silence.start / (double)SampleRate);
                silencePoints.Add(silenceTime);
            }
        }

        // 移除间隔过近的停顿点
        silencePoints = RemoveCloseTimeSpans(silencePoints, 2, totalTime); // 间隔小于3秒的移除
        return silencePoints.Take(TopCount).OrderBy(t => t).ToList();
    }

    public static List<TimeSpan> RemoveCloseTimeSpans(List<TimeSpan> timeSpans, int seconds, TimeSpan totalTime)
    {
        List<TimeSpan> result = new List<TimeSpan>(); // 用于存储符合条件的时间点
        foreach (var currentTime in timeSpans)
        {
            // 遍历已处理过的时间点，检查时间差是否小于指定的阈值（比如3秒）
            var shouldAdd = Math.Abs((currentTime - TimeSpan.Zero).TotalSeconds) > seconds &&
                            Math.Abs((currentTime - totalTime).TotalSeconds) > seconds &&
                            result.All(r =>
                                Math.Abs((currentTime - r).TotalSeconds) > seconds
                            );
            if (shouldAdd)
            {
                result.Add(currentTime); // 如果时间差不小于阈值，添加到结果中
            }
        }

        return result;
    }
}