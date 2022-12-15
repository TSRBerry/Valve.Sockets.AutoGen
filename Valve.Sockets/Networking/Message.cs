using System.Runtime.InteropServices;

namespace Valve.Sockets.Networking;

[StructLayout(LayoutKind.Sequential)]
public struct Message {
    public IntPtr data;
    public int length;
    public uint connection;
    public Identity identity;
    public long connectionUserData;
    public long timeReceived;
    public long messageNumber;
    internal IntPtr freeData;
    internal IntPtr release;
    public int channel;
    public int flags;
    public long userData;

    public void CopyTo(byte[] destination) {
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        Marshal.Copy(data, destination, 0, length);
    }
}