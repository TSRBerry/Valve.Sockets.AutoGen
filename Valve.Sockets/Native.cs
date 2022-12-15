using System.Runtime.InteropServices;
using Valve.Sockets.Networking;
using Valve.Sockets.Types;
using Valve.Sockets.Types.Configuration;
using Valve.Sockets.Types.Connection;

namespace Valve.Sockets;

internal static partial class Native {
    private const string NativeLibrary = "GameNetworkingSockets";

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GameNetworkingSockets_Init(IntPtr identity, [MarshalAs(UnmanagedType.LPStr)] string errorMessage);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GameNetworkingSockets_Init(ref Identity identity, [MarshalAs(UnmanagedType.LPStr)] string errorMessage);

    [LibraryImport(NativeLibrary)]
    internal static partial void GameNetworkingSockets_Kill();

    [LibraryImport(NativeLibrary)]
    internal static partial IntPtr SteamAPI_SteamNetworkingSockets_v009();

    [LibraryImport(NativeLibrary)]
    internal static partial IntPtr SteamAPI_SteamNetworkingUtils_v003();

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr sockets, ref Address address, int configurationsCount, IntPtr configurations);

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(IntPtr sockets, ref Address address, int configurationsCount, Configuration[] configurations);

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr sockets, ref Address address, int configurationsCount, IntPtr configurations);

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(IntPtr sockets, ref Address address, int configurationsCount, Configuration[] configurations);

    [LibraryImport(NativeLibrary)]
    internal static partial Result SteamAPI_ISteamNetworkingSockets_AcceptConnection(IntPtr sockets, uint connection);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_CloseConnection(IntPtr sockets, uint peer, int reason, [MarshalAs(UnmanagedType.LPStr)] string debug, [MarshalAs(UnmanagedType.Bool)] bool enableLinger);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_CloseListenSocket(IntPtr sockets, uint socket);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(IntPtr sockets, uint peer, long userData);

    [LibraryImport(NativeLibrary)]
    internal static partial long SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(IntPtr sockets, uint peer);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_ISteamNetworkingSockets_SetConnectionName(IntPtr sockets, uint peer, [MarshalAs(UnmanagedType.LPStr)] string name);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_GetConnectionName(IntPtr sockets, uint peer, [MarshalAs(UnmanagedType.LPStr)] string name, int maxLength);

    [LibraryImport(NativeLibrary)]
    internal static partial Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr sockets, uint connection, IntPtr data, uint length, SendFlags flags, IntPtr outMessageNumber);

    [LibraryImport(NativeLibrary)]
    internal static partial Result SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(IntPtr sockets, uint connection, byte[] data, uint length, SendFlags flags, IntPtr outMessageNumber);

    [LibraryImport(NativeLibrary)]
    internal static partial Result SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(IntPtr sockets, uint connection);

    [LibraryImport(NativeLibrary)]
    internal static partial int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(IntPtr sockets, uint connection, IntPtr[] messages, int maxMessages);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(IntPtr sockets, uint connection, ref Info info);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(IntPtr sockets, uint connection, ref Status status);

    [LibraryImport(NativeLibrary)]
    internal static partial int SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(IntPtr sockets, uint connection, [MarshalAs(UnmanagedType.LPStr)] string status, int statusLength);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(IntPtr sockets, uint socket, ref Address address);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_ISteamNetworkingSockets_RunCallbacks(IntPtr sockets);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_CreateSocketPair(IntPtr sockets, uint connectionLeft, uint connectionRight, [MarshalAs(UnmanagedType.Bool)] bool useNetworkLoopback, ref Identity identityLeft, ref Identity identityRight);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_GetIdentity(IntPtr sockets, ref Identity identity);

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_ISteamNetworkingSockets_CreatePollGroup(IntPtr sockets);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(IntPtr sockets, uint pollGroup);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(IntPtr sockets, uint connection, uint pollGroup);

    [LibraryImport(NativeLibrary)]
    internal static partial int SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(IntPtr sockets, uint pollGroup, IntPtr[] messages, int maxMessages);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref Address address, byte[] ip, ushort port);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref Address address, uint ip, ushort port);

    [LibraryImport(NativeLibrary)]
    internal static partial uint SteamAPI_SteamNetworkingIPAddr_GetIPv4(ref Address address);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref Address address, ushort port);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_SteamNetworkingIPAddr_IsLocalHost(ref Address address);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_SteamNetworkingIPAddr_IsEqualTo(ref Address address, ref Address other);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_SteamNetworkingIdentity_IsInvalid(ref Identity identity);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_SteamNetworkingIdentity_SetSteamID64(ref Identity identity, ulong steamID);

    [LibraryImport(NativeLibrary)]
    internal static partial ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(ref Identity identity);

    [LibraryImport(NativeLibrary)]
    internal static partial long SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr utils);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr utils, IntPtr callback);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingUtils_SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr utils, StatusCallback callback);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr utils, DebugType detailLevel, IntPtr callback);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr utils, DebugType detailLevel, DebugCallback callback);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(IntPtr utils, Value configurationValue, Scope configurationScope, IntPtr scopeObject, DataType dataType, IntPtr value);

    [LibraryImport(NativeLibrary)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr utils, Configuration configuration, Scope configurationScope, IntPtr scopeObject);

    [LibraryImport(NativeLibrary)]
    internal static partial ValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(IntPtr utils, Value configurationValue, Scope configurationScope, IntPtr scopeObject, ref DataType dataType, ref IntPtr result, ref IntPtr resultLength);

    [LibraryImport(NativeLibrary)]
    internal static partial Value SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(IntPtr utils);

    [LibraryImport(NativeLibrary)]
    internal static partial void SteamAPI_SteamNetworkingMessage_t_Release(IntPtr nativeMessage);
}