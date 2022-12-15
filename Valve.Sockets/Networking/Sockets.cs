using System.Runtime.CompilerServices;
using Valve.Sockets.Types;
using Valve.Sockets.Types.Configuration;
using Valve.Sockets.Types.Connection;

namespace Valve.Sockets.Networking;

public class Sockets {
    private IntPtr nativeSockets;

    public Sockets() {
        nativeSockets = Native.SteamAPI_SteamNetworkingSockets_v009();

        if (nativeSockets == IntPtr.Zero)
            throw new InvalidOperationException("Networking sockets not created");
    }

    public uint CreateListenSocket(ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(nativeSockets, ref address, 0, IntPtr.Zero);
    }

    public uint CreateListenSocket(ref Address address, Configuration[] configurations) {
        if (configurations == null)
            throw new ArgumentNullException("configurations");

        return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(nativeSockets, ref address, configurations.Length, configurations);
    }

    public uint Connect(ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(nativeSockets, ref address, 0, IntPtr.Zero);
    }

    public uint Connect(ref Address address, Configuration[] configurations) {
        if (configurations == null)
            throw new ArgumentNullException("configurations");

        return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(nativeSockets, ref address, configurations.Length, configurations);
    }

    public Result AcceptConnection(uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_AcceptConnection(nativeSockets, connection);
    }

    public bool CloseConnection(uint connection) {
        return CloseConnection(connection, 0, string.Empty, false);
    }

    public bool CloseConnection(uint connection, int reason, string debug, bool enableLinger) {
        if (debug.Length > Library.maxCloseMessageLength)
            throw new ArgumentOutOfRangeException("debug");

        return Native.SteamAPI_ISteamNetworkingSockets_CloseConnection(nativeSockets, connection, reason, debug, enableLinger);
    }

    public bool CloseListenSocket(uint socket) {
        return Native.SteamAPI_ISteamNetworkingSockets_CloseListenSocket(nativeSockets, socket);
    }

    public bool SetConnectionUserData(uint peer, long userData) {
        return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(nativeSockets, peer, userData);
    }

    public long GetConnectionUserData(uint peer) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(nativeSockets, peer);
    }

    public void SetConnectionName(uint peer, string name) {
        Native.SteamAPI_ISteamNetworkingSockets_SetConnectionName(nativeSockets, peer, name);
    }

    public bool GetConnectionName(uint peer, string name, int maxLength) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionName(nativeSockets, peer, name, maxLength);
    }

    public Result SendMessageToConnection(uint connection, IntPtr data, uint length) {
        return SendMessageToConnection(connection, data, length, SendFlags.Unreliable);
    }

    public Result SendMessageToConnection(uint connection, IntPtr data, uint length, SendFlags flags) {
        return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, length, flags, IntPtr.Zero);
    }

    public Result SendMessageToConnection(uint connection, IntPtr data, int length, SendFlags flags) {
        return SendMessageToConnection(connection, data, (uint)length, flags);
    }

    public Result SendMessageToConnection(uint connection, byte[] data) {
        if (data == null)
            throw new ArgumentNullException("data");

        return SendMessageToConnection(connection, data, data.Length, SendFlags.Unreliable);
    }

    public Result SendMessageToConnection(uint connection, byte[] data, SendFlags flags) {
        if (data == null)
            throw new ArgumentNullException("data");

        return SendMessageToConnection(connection, data, data.Length, flags);
    }

    public Result SendMessageToConnection(uint connection, byte[] data, int length, SendFlags flags) {
        if (data == null)
            throw new ArgumentNullException("data");

        return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, (uint)length, flags, IntPtr.Zero);
    }

    public Result FlushMessagesOnConnection(uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(nativeSockets, connection);
    }

    public bool GetConnectionInfo(uint connection, ref Info info) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(nativeSockets, connection, ref info);
    }

    public bool GetQuickConnectionStatus(uint connection, ref Status status) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus(nativeSockets, connection, ref status);
    }

    public int GetDetailedConnectionStatus(uint connection, string status, int statusLength) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(nativeSockets, connection, status, statusLength);
    }

    public bool GetListenSocketAddress(uint socket, ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(nativeSockets, socket, ref address);
    }

    public bool CreateSocketPair(uint connectionLeft, uint connectionRight, bool useNetworkLoopback, ref Identity identityLeft, ref Identity identityRight) {
        return Native.SteamAPI_ISteamNetworkingSockets_CreateSocketPair(nativeSockets, connectionLeft, connectionRight, useNetworkLoopback, ref identityLeft, ref identityRight);
    }

    public bool GetIdentity(ref Identity identity) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetIdentity(nativeSockets, ref identity);
    }

    public uint CreatePollGroup() {
        return Native.SteamAPI_ISteamNetworkingSockets_CreatePollGroup(nativeSockets);
    }

    public bool DestroyPollGroup(uint pollGroup) {
        return Native.SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(nativeSockets, pollGroup);
    }

    public bool SetConnectionPollGroup(uint pollGroup, uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(nativeSockets, connection, pollGroup);
    }

    public void RunCallbacks() {
        Native.SteamAPI_ISteamNetworkingSockets_RunCallbacks(nativeSockets);
    }

#if VALVESOCKETS_SPAN
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void ReceiveMessagesOnConnection(uint connection, MessageCallback callback, int maxMessages) {
				if (maxMessages > Library.maxMessagesPerBatch)
					throw new ArgumentOutOfRangeException("maxMessages");

				IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
				int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(nativeSockets, connection, nativeMessages, maxMessages);

				for (int i = 0; i < messagesCount; i++) {
					Span<Message> message;

					unsafe {
						message = new Span<Message>((void*)nativeMessages[i], 1);
					}

					callback(in message[0]);

					Native.SteamAPI_SteamNetworkingMessage_t_Release(nativeMessages[i]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void ReceiveMessagesOnPollGroup(uint pollGroup, MessageCallback callback, int maxMessages) {
				if (maxMessages > Library.maxMessagesPerBatch)
					throw new ArgumentOutOfRangeException("maxMessages");

				IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
				int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(nativeSockets, pollGroup, nativeMessages, maxMessages);

				for (int i = 0; i < messagesCount; i++) {
					Span<Message> message;

					unsafe {
						message = new Span<Message>((void*)nativeMessages[i], 1);
					}

					callback(in message[0]);

					Native.SteamAPI_SteamNetworkingMessage_t_Release(nativeMessages[i]);
				}
			}
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReceiveMessagesOnConnection(uint connection, Message[] messages, int maxMessages) {
        if (messages == null)
            throw new ArgumentNullException("messages");

        if (maxMessages > Library.maxMessagesPerBatch)
            throw new ArgumentOutOfRangeException("maxMessages");

        IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
        int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection(nativeSockets, connection, nativeMessages, maxMessages);

        for (int i = 0; i < messagesCount; i++) {
            messages[i] = (Message)Marshal.PtrToStructure(nativeMessages[i], typeof(Message));
            messages[i].release = nativeMessages[i];
        }

        return messagesCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReceiveMessagesOnPollGroup(uint pollGroup, Message[] messages, int maxMessages) {
        if (messages == null)
            throw new ArgumentNullException("messages");

        if (maxMessages > Library.maxMessagesPerBatch)
            throw new ArgumentOutOfRangeException("maxMessages");

        IntPtr[] nativeMessages = ArrayPool.GetPointerBuffer();
        int messagesCount = Native.SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup(nativeSockets, pollGroup, nativeMessages, maxMessages);

        for (int i = 0; i < messagesCount; i++) {
            messages[i] = (Message)Marshal.PtrToStructure(nativeMessages[i], typeof(Message));
            messages[i].release = nativeMessages[i];
        }

        return messagesCount;
    }
#endif
}