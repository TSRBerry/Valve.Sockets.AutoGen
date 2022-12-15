using System.Runtime.InteropServices;

namespace Valve.Sockets.Types.Connection;

[StructLayout(LayoutKind.Sequential)]
public struct Status {
    public State state;
    public int ping;
    public float connectionQualityLocal;
    public float connectionQualityRemote;
    public float outPacketsPerSecond;
    public float outBytesPerSecond;
    public float inPacketsPerSecond;
    public float inBytesPerSecond;
    public int sendRateBytesPerSecond;
    public int pendingUnreliable;
    public int pendingReliable;
    public int sentUnackedReliable;
    public long queueTime;
    private Array16<uint> reserved;
}