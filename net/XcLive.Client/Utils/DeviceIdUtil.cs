using System.Net.NetworkInformation;

namespace Frame.Utils;

public class DeviceIdUtil
{
    public static string GetMacAddress()
    {
        string macAddress = string.Empty;
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface nic in nics)
        {
            if (nic.OperationalStatus == OperationalStatus.Up)
            {
                macAddress = nic.GetPhysicalAddress().ToString();
                break;
            }
        }

        return macAddress;
    }
}