using NAudio.CoreAudioApi;
using Newtonsoft.Json;

namespace Frame.Utils;

public class SoundCardManager
{
    private readonly MMDeviceEnumerator _enumerator = new();

    // 获取所有输出设备（声卡）
    public List<SoundCardInfo> GetAllOutputDevices()
    {
        return _enumerator
            .EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .Select(d => new SoundCardInfo
            {
                Id = d.ID,
                Name = d.FriendlyName,
                Volume = (int)(d.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                IsDefault = d.ID == GetDefaultOutputDevice().Id
            })
            .ToList();
    }

    // 获取默认设备
    public SoundCardInfo GetDefaultOutputDevice()
    {
        var device = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        return new SoundCardInfo
        {
            Id = device.ID,
            Name = device.FriendlyName,
            Volume = (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
            IsDefault = true
        };
    }

    // 获取指定设备音量（0~100）
    public int GetVolume(string deviceId)
    {
        var device = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .FirstOrDefault(d => d.ID == deviceId);
        return device == null ? 0 : (int)(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
    }

    // 设置指定设备音量（0~100）
    public void SetVolume(string deviceId, int volume)
    {
        var device = _enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            .FirstOrDefault(d => d.ID == deviceId);
        if (device != null)
        {
            float vol = Math.Clamp(volume, 0, 100) / 100f;
            device.AudioEndpointVolume.MasterVolumeLevelScalar = vol;
        }
    }
}

// 声卡信息类（音量为 int）
public class SoundCardInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Volume { get; set; }      // 0 ~ 100
    public bool IsDefault { get; set; }
}