using System.Runtime.InteropServices;
using Valve.Sockets.Types;

namespace Valve.Sockets.Networking;

[StructLayout(LayoutKind.Explicit, Size = 136)]
public struct Identity {
    [FieldOffset(0)]
    public IdentityType type;

    public bool IsInvalid => Native.SteamAPI_SteamNetworkingIdentity_IsInvalid(ref this);

    public ulong GetSteamID() {
        return Native.SteamAPI_SteamNetworkingIdentity_GetSteamID64(ref this);
    }

    public void SetSteamID(ulong steamID) {
        Native.SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref this, steamID);
    }
}