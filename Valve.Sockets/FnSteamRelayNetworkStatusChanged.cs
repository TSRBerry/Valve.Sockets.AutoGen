using System.Runtime.InteropServices;

namespace Valve.Sockets
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void FnSteamRelayNetworkStatusChanged(SteamRelayNetworkStatus_t* param0);
}