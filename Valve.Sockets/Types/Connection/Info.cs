using System.Runtime.InteropServices;
using System.Text;
using Valve.Sockets.Networking;

namespace Valve.Sockets.Types.Connection;

[StructLayout(LayoutKind.Sequential)]
public struct Info {
    public Identity identity;
    public long userData;
    public uint listenSocket;
    public Address address;
    private ushort pad;
    private uint popRemote;
    private uint popRelay;
    public State state;
    public int endReason;
    private Array128<byte> _endDebug;
    private Array128<byte> _connectionDescription;
    private Array64<uint> reserved;

    public string endDebug => Encoding.Default.GetString(_endDebug.AsSpan());
    public string connectionDescription => Encoding.Default.GetString(_connectionDescription.AsSpan());
}