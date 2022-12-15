using System.Net;
using System.Net.Sockets;

namespace Valve.Sockets;

public static class Extensions {
    public static uint ParseIPv4(this string ip) {
        IPAddress address = default;

        if (IPAddress.TryParse(ip, out address)) {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                throw new Exception("Incorrect format of an IPv4 address");
        }

        byte[] bytes = address.GetAddressBytes();

        Array.Reverse(bytes);

        return BitConverter.ToUInt32(bytes, 0);
    }

    public static byte[] ParseIPv6(this string ip) {
        IPAddress address = default;

        if (IPAddress.TryParse(ip, out address)) {
            if (address.AddressFamily != AddressFamily.InterNetworkV6)
                throw new Exception("Incorrect format of an IPv6 address");
        }

        return address.GetAddressBytes();
    }

    public static string ParseIP(this Array16<byte> ip) {
        IPAddress address = new IPAddress(ip.AsSpan());
        string converted = address.ToString();

        if (converted.Length > 7 && converted.Remove(7) == "::ffff:") {
            Address ipv4 = default(Address);

            ipv4.IP = ip;

            byte[] bytes = BitConverter.GetBytes(Native.SteamAPI_SteamNetworkingIPAddr_GetIPv4(ref ipv4));

            Array.Reverse(bytes);

            address = new IPAddress(bytes);
        }

        return address.ToString();
    }
}