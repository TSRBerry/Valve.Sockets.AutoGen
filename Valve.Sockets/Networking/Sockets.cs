using System.Runtime.CompilerServices;
using Valve.Sockets.Types;
using Valve.Sockets.Types.Configuration;
using Valve.Sockets.Types.Connection;

namespace Valve.Sockets.Networking;

/// Lower level networking API.
///
/// - Connection-oriented API (like TCP, not UDP).  When sending and receiving
///   messages, a connection handle is used.  (For a UDP-style interface, where
///   the peer is identified by their address with each send/recv call, see
///   ISteamNetworkingMessages.)  The typical pattern is for a "server" to "listen"
///   on a "listen socket."  A "client" will "connect" to the server, and the
///   server will "accept" the connection.  If you have a symmetric situation
///   where either peer may initiate the connection and server/client roles are
///   not clearly defined, check out k_ESteamNetworkingConfig_SymmetricConnect.
/// - But unlike TCP, it's message-oriented, not stream-oriented.
/// - Mix of reliable and unreliable messages
/// - Fragmentation and reassembly
/// - Supports connectivity over plain UDP
/// - Also supports SDR ("Steam Datagram Relay") connections, which are
///   addressed by the identity of the peer.  There is a "P2P" use case and
///   a "hosted dedicated server" use case.
///
/// Note that neither of the terms "connection" nor "socket" necessarily correspond
/// one-to-one with an underlying UDP socket.  An attempt has been made to
/// keep the semantics as similar to the standard socket model when appropriate,
/// but some deviations do exist.
///
/// See also: ISteamNetworkingMessages, the UDP-style interface.  This API might be
/// easier to use, especially when porting existing UDP code.
public class Sockets {
    private IntPtr nativeSockets;

    public Sockets() {
        nativeSockets = Native.SteamAPI_SteamNetworkingSockets_v009();

        if (nativeSockets == IntPtr.Zero)
            throw new InvalidOperationException("Networking sockets not created");
    }

    /// Creates a "server" socket that listens for clients to connect to by
    /// calling ConnectByIPAddress, over ordinary UDP (IPv4 or IPv6)
    ///
    /// You must select a specific local port to listen on and set it
    /// the port field of the local address.
    ///
    /// Usually you will set the IP portion of the address to zero (SteamNetworkingIPAddr::Clear()).
    /// This means that you will not bind to any particular local interface (i.e. the same
    /// as INADDR_ANY in plain socket code).  Furthermore, if possible the socket will be bound
    /// in "dual stack" mode, which means that it can accept both IPv4 and IPv6 client connections.
    /// If you really do wish to bind a particular interface, then set the local address to the
    /// appropriate IPv4 or IPv6 IP.
    ///
    /// If you need to set any initial config options, pass them here.  See
    /// SteamNetworkingConfigValue_t for more about why this is preferable to
    /// setting the options "immediately" after creation.
    ///
    /// When a client attempts to connect, a SteamNetConnectionStatusChangedCallback_t
    /// will be posted.  The connection will be in the connecting state.
    public uint CreateListenSocket(ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(nativeSockets, ref address, 0, IntPtr.Zero);
    }

    /// <inheritdoc cref="CreateListenSocket(ref Valve.Sockets.Address)"/>
    public uint CreateListenSocket(ref Address address, Configuration[] configurations) {
        if (configurations == null)
            throw new ArgumentNullException(nameof(configurations));

        return Native.SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP(nativeSockets, ref address, configurations.Length, configurations);
    }

    /// Creates a connection and begins talking to a "server" over UDP at the
    /// given IPv4 or IPv6 address.  The remote host must be listening with a
    /// matching call to CreateListenSocketIP on the specified port.
    ///
    /// A SteamNetConnectionStatusChangedCallback_t callback will be triggered when we start
    /// connecting, and then another one on either timeout or successful connection.
    ///
    /// If the server does not have any identity configured, then their network address
    /// will be the only identity in use.  Or, the network host may provide a platform-specific
    /// identity with or without a valid certificate to authenticate that identity.  (These
    /// details will be contained in the SteamNetConnectionStatusChangedCallback_t.)  It's
    /// up to your application to decide whether to allow the connection.
    ///
    /// By default, all connections will get basic encryption sufficient to prevent
    /// casual eavesdropping.  But note that without certificates (or a shared secret
    /// distributed through some other out-of-band mechanism), you don't have any
    /// way of knowing who is actually on the other end, and thus are vulnerable to
    /// man-in-the-middle attacks.
    ///
    /// If you need to set any initial config options, pass them here.  See
    /// SteamNetworkingConfigValue_t for more about why this is preferable to
    /// setting the options "immediately" after creation.
    public uint Connect(ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(nativeSockets, ref address, 0, IntPtr.Zero);
    }

    /// <inheritdoc cref="Connect(ref Valve.Sockets.Address)"/>
    public uint Connect(ref Address address, Configuration[] configurations) {
        if (configurations == null)
            throw new ArgumentNullException(nameof(configurations));

        return Native.SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress(nativeSockets, ref address, configurations.Length, configurations);
    }

    /// Accept an incoming connection that has been received on a listen socket.
	///
	/// When a connection attempt is received (perhaps after a few basic handshake
	/// packets have been exchanged to prevent trivial spoofing), a connection interface
	/// object is created in the k_ESteamNetworkingConnectionState_Connecting state
	/// and a SteamNetConnectionStatusChangedCallback_t is posted.  At this point, your
	/// application MUST either accept or close the connection.  (It may not ignore it.)
	/// Accepting the connection will transition it either into the connected state,
	/// or the finding route state, depending on the connection type.
	///
	/// You should take action within a second or two, because accepting the connection is
	/// what actually sends the reply notifying the client that they are connected.  If you
	/// delay taking action, from the client's perspective it is the same as the network
	/// being unresponsive, and the client may timeout the connection attempt.  In other
	/// words, the client cannot distinguish between a delay caused by network problems
	/// and a delay caused by the application.
	///
	/// This means that if your application goes for more than a few seconds without
	/// processing callbacks (for example, while loading a map), then there is a chance
	/// that a client may attempt to connect in that interval and fail due to timeout.
	///
	/// If the application does not respond to the connection attempt in a timely manner,
	/// and we stop receiving communication from the client, the connection attempt will
	/// be timed out locally, transitioning the connection to the
	/// k_ESteamNetworkingConnectionState_ProblemDetectedLocally state.  The client may also
	/// close the connection before it is accepted, and a transition to the
	/// k_ESteamNetworkingConnectionState_ClosedByPeer is also possible depending the exact
	/// sequence of events.
	///
	/// Returns k_EResultInvalidParam if the handle is invalid.
	/// Returns k_EResultInvalidState if the connection is not in the appropriate state.
	/// (Remember that the connection state could change in between the time that the
	/// notification being posted to the queue and when it is received by the application.)
	///
	/// A note about connection configuration options.  If you need to set any configuration
	/// options that are common to all connections accepted through a particular listen
	/// socket, consider setting the options on the listen socket, since such options are
	/// inherited automatically.  If you really do need to set options that are connection
	/// specific, it is safe to set them on the connection before accepting the connection.
    public Result AcceptConnection(uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_AcceptConnection(nativeSockets, connection);
    }

    /// <inheritdoc cref="CloseConnection(uint, int, string, bool)"/>
    public bool CloseConnection(uint connection) {
        return CloseConnection(connection, 0, string.Empty, false);
    }

    /// Disconnects from the remote host and invalidates the connection handle.
    /// Any unread data on the connection is discarded.
    ///
    /// nReason is an application defined code that will be received on the other
    /// end and recorded (when possible) in backend analytics.  The value should
    /// come from a restricted range.  (See ESteamNetConnectionEnd.)  If you don't need
    /// to communicate any information to the remote host, and do not want analytics to
    /// be able to distinguish "normal" connection terminations from "exceptional" ones,
    /// You may pass zero, in which case the generic value of
    /// k_ESteamNetConnectionEnd_App_Generic will be used.
    ///
    /// pszDebug is an optional human-readable diagnostic string that will be received
    /// by the remote host and recorded (when possible) in backend analytics.
    ///
    /// If you wish to put the socket into a "linger" state, where an attempt is made to
    /// flush any remaining sent data, use bEnableLinger=true.  Otherwise reliable data
    /// is not flushed.
    ///
    /// If the connection has already ended and you are just freeing up the
    /// connection interface, the reason code, debug string, and linger flag are
    /// ignored.
    public bool CloseConnection(uint connection, int reason, string debug, bool enableLinger) {
        if (debug.Length > Library.maxCloseMessageLength)
            throw new ArgumentOutOfRangeException(nameof(debug));

        return Native.SteamAPI_ISteamNetworkingSockets_CloseConnection(nativeSockets, connection, reason, debug, enableLinger);
    }

    /// Destroy a listen socket.  All the connections that were accepting on the listen
    /// socket are closed ungracefully.
    public bool CloseListenSocket(uint socket) {
        return Native.SteamAPI_ISteamNetworkingSockets_CloseListenSocket(nativeSockets, socket);
    }

    /// Set connection user data.  the data is returned in the following places
    /// - You can query it using GetConnectionUserData.
    /// - The SteamNetworkingmessage_t structure.
    /// - The SteamNetConnectionInfo_t structure.
    ///   (Which is a member of SteamNetConnectionStatusChangedCallback_t -- but see WARNINGS below!!!!)
    ///
    /// Do you need to set this atomically when the connection is created?
    /// See k_ESteamNetworkingConfig_ConnectionUserData.
    ///
    /// WARNING: Be *very careful* when using the value provided in callbacks structs.
    /// Callbacks are queued, and the value that you will receive in your
    /// callback is the userdata that was effective at the time the callback
    /// was queued.  There are subtle race conditions that can hapen if you
    /// don't understand this!
    ///
    /// If any incoming messages for this connection are queued, the userdata
    /// field is updated, so that when when you receive messages (e.g. with
    /// ReceiveMessagesOnConnection), they will always have the very latest
    /// userdata.  So the tricky race conditions that can happen with callbacks
    /// do not apply to retrieving messages.
    ///
    /// Returns false if the handle is invalid.
    public bool SetConnectionUserData(uint peer, long userData) {
        return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionUserData(nativeSockets, peer, userData);
    }

    /// Fetch connection user data.  Returns -1 if handle is invalid
    /// or if you haven't set any userdata on the connection.
    public long GetConnectionUserData(uint peer) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionUserData(nativeSockets, peer);
    }

    /// Set a name for the connection, used mostly for debugging
    public void SetConnectionName(uint peer, string name) {
        Native.SteamAPI_ISteamNetworkingSockets_SetConnectionName(nativeSockets, peer, name);
    }

    /// Fetch connection name.  Returns false if handle is invalid
    public bool GetConnectionName(uint peer, string name, int maxLength) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionName(nativeSockets, peer, name, maxLength);
    }

    /// <inheritdoc cref="SendMessageToConnection(uint,nint,uint,SendFlags)"/>
    public Result SendMessageToConnection(uint connection, IntPtr data, uint length) {
        return SendMessageToConnection(connection, data, length, SendFlags.Unreliable);
    }

    /// Send a message to the remote host on the specified connection.
	///
	/// nSendFlags determines the delivery guarantees that will be provided,
	/// when data should be buffered, etc.  E.g. k_nSteamNetworkingSend_Unreliable
	///
	/// Note that the semantics we use for messages are not precisely
	/// the same as the semantics of a standard "stream" socket.
	/// (SOCK_STREAM)  For an ordinary stream socket, the boundaries
	/// between chunks are not considered relevant, and the sizes of
	/// the chunks of data written will not necessarily match up to
	/// the sizes of the chunks that are returned by the reads on
	/// the other end.  The remote host might read a partial chunk,
	/// or chunks might be coalesced.  For the message semantics
	/// used here, however, the sizes WILL match.  Each send call
	/// will match a successful read call on the remote host
	/// one-for-one.  If you are porting existing stream-oriented
	/// code to the semantics of reliable messages, your code should
	/// work the same, since reliable message semantics are more
	/// strict than stream semantics.  The only caveat is related to
	/// performance: there is per-message overhead to retain the
	/// message sizes, and so if your code sends many small chunks
	/// of data, performance will suffer. Any code based on stream
	/// sockets that does not write excessively small chunks will
	/// work without any changes.
	///
	/// The pOutMessageNumber is an optional pointer to receive the
	/// message number assigned to the message, if sending was successful.
	///
	/// Returns:
	/// - k_EResultInvalidParam: invalid connection handle, or the individual message is too big.
	///   (See k_cbMaxSteamNetworkingSocketsMessageSizeSend)
	/// - k_EResultInvalidState: connection is in an invalid state
	/// - k_EResultNoConnection: connection has ended
	/// - k_EResultIgnored: You used k_nSteamNetworkingSend_NoDelay, and the message was dropped because
	///   we were not ready to send it.
	/// - k_EResultLimitExceeded: there was already too much data queued to be sent.
	///   (See k_ESteamNetworkingConfig_SendBufferSize)
    public Result SendMessageToConnection(uint connection, IntPtr data, uint length, SendFlags flags) {
        return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, length, flags, IntPtr.Zero);
    }

    /// <inheritdoc cref="SendMessageToConnection(uint,nint,uint, SendFlags)"/>
    public Result SendMessageToConnection(uint connection, IntPtr data, int length, SendFlags flags) {
        return SendMessageToConnection(connection, data, (uint)length, flags);
    }

    /// <inheritdoc cref="SendMessageToConnection(uint,nint,uint, SendFlags)"/>
    public Result SendMessageToConnection(uint connection, byte[] data) {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return SendMessageToConnection(connection, data, data.Length, SendFlags.Unreliable);
    }

    /// <inheritdoc cref="SendMessageToConnection(uint,nint,uint, SendFlags)"/>
    public Result SendMessageToConnection(uint connection, byte[] data, SendFlags flags) {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return SendMessageToConnection(connection, data, data.Length, flags);
    }

    /// <inheritdoc cref="SendMessageToConnection(uint,nint,uint, SendFlags)"/>
    public Result SendMessageToConnection(uint connection, byte[] data, int length, SendFlags flags) {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return Native.SteamAPI_ISteamNetworkingSockets_SendMessageToConnection(nativeSockets, connection, data, (uint)length, flags, IntPtr.Zero);
    }

    /// Flush any messages waiting on the Nagle timer and send them
    /// at the next transmission opportunity (often that means right now).
    ///
    /// If Nagle is enabled (it's on by default) then when calling
    /// SendMessageToConnection the message will be buffered, up to the Nagle time
    /// before being sent, to merge small messages into the same packet.
    /// (See k_ESteamNetworkingConfig_NagleTime)
    ///
    /// Returns:
    /// k_EResultInvalidParam: invalid connection handle
    /// k_EResultInvalidState: connection is in an invalid state
    /// k_EResultNoConnection: connection has ended
    /// k_EResultIgnored: We weren't (yet) connected, so this operation has no effect.
    public Result FlushMessagesOnConnection(uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection(nativeSockets, connection);
    }

    /// Returns basic information about the high-level state of the connection.
    public bool GetConnectionInfo(uint connection, ref Info info) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionInfo(nativeSockets, connection, ref info);
    }

    /// Returns a small set of information about the real-time state of the connection
    /// and the queue status of each lane.
    ///
    /// - pStatus may be NULL if the information is not desired.  (E.g. you are only interested
    ///   in the lane information.)
    /// - On entry, nLanes specifies the length of the pLanes array.  This may be 0
    ///   if you do not wish to receive any lane data.  It's OK for this to be smaller than
    ///   the total number of configured lanes.
    /// - pLanes points to an array that will receive lane-specific info.  It can be NULL
    ///   if this is not needed.
    ///
    /// Return value:
    /// - k_EResultNoConnection - connection handle is invalid or connection has been closed.
    /// - k_EResultInvalidParam - nLanes is bad
    public bool GetConnectionRealTimeStatus(uint connection, ref Status status) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetConnectionRealTimeStatus(nativeSockets, connection, ref status);
    }

    /// Returns detailed connection stats in text format.  Useful
    /// for dumping to a log, etc.
    ///
    /// Returns:
    /// -1 failure (bad connection handle)
    /// 0 OK, your buffer was filled in and '\0'-terminated
    /// >0 Your buffer was either nullptr, or it was too small and the text got truncated.
    ///    Try again with a buffer of at least N bytes.
    public int GetDetailedConnectionStatus(uint connection, string status, int statusLength) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus(nativeSockets, connection, status, statusLength);
    }

    /// Returns local IP and port that a listen socket created using CreateListenSocketIP is bound to.
    ///
    /// An IPv6 address of ::0 means "any IPv4 or IPv6"
    /// An IPv6 address of ::ffff:0000:0000 means "any IPv4"
    public bool GetListenSocketAddress(uint socket, ref Address address) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress(nativeSockets, socket, ref address);
    }

    /// Create a pair of connections that are talking to each other, e.g. a loopback connection.
    /// This is very useful for testing, or so that your client/server code can work the same
    /// even when you are running a local "server".
    ///
    /// The two connections will immediately be placed into the connected state, and no callbacks
    /// will be posted immediately.  After this, if you close either connection, the other connection
    /// will receive a callback, exactly as if they were communicating over the network.  You must
    /// close *both* sides in order to fully clean up the resources!
    ///
    /// By default, internal buffers are used, completely bypassing the network, the chopping up of
    /// messages into packets, encryption, copying the payload, etc.  This means that loopback
    /// packets, by default, will not simulate lag or loss.  Passing true for bUseNetworkLoopback will
    /// cause the socket pair to send packets through the local network loopback device (127.0.0.1)
    /// on ephemeral ports.  Fake lag and loss are supported in this case, and CPU time is expended
    /// to encrypt and decrypt.
    ///
    /// If you wish to assign a specific identity to either connection, you may pass a particular
    /// identity.  Otherwise, if you pass nullptr, the respective connection will assume a generic
    /// "localhost" identity.  If you use real network loopback, this might be translated to the
    /// actual bound loopback port.  Otherwise, the port will be zero.
    public bool CreateSocketPair(uint connectionLeft, uint connectionRight, bool useNetworkLoopback, ref Identity identityLeft, ref Identity identityRight) {
        return Native.SteamAPI_ISteamNetworkingSockets_CreateSocketPair(nativeSockets, connectionLeft, connectionRight, useNetworkLoopback, ref identityLeft, ref identityRight);
    }

    /// Get the identity assigned to this interface.
    /// E.g. on Steam, this is the user's SteamID, or for the gameserver interface, the SteamID assigned
    /// to the gameserver.  Returns false and sets the result to an invalid identity if we don't know
    /// our identity yet.  (E.g. GameServer has not logged in.  On Steam, the user will know their SteamID
    /// even if they are not signed into Steam.)
    public bool GetIdentity(ref Identity identity) {
        return Native.SteamAPI_ISteamNetworkingSockets_GetIdentity(nativeSockets, ref identity);
    }

    /// Create a new poll group.
    ///
    /// You should destroy the poll group when you are done using DestroyPollGroup
    public uint CreatePollGroup() {
        return Native.SteamAPI_ISteamNetworkingSockets_CreatePollGroup(nativeSockets);
    }

    /// Destroy a poll group created with CreatePollGroup().
    ///
    /// If there are any connections in the poll group, they are removed from the group,
    /// and left in a state where they are not part of any poll group.
    /// Returns false if passed an invalid poll group handle.
    public bool DestroyPollGroup(uint pollGroup) {
        return Native.SteamAPI_ISteamNetworkingSockets_DestroyPollGroup(nativeSockets, pollGroup);
    }

    /// Assign a connection to a poll group.  Note that a connection may only belong to a
    /// single poll group.  Adding a connection to a poll group implicitly removes it from
    /// any other poll group it is in.
    ///
    /// You can pass k_HSteamNetPollGroup_Invalid to remove a connection from its current
    /// poll group without adding it to a new poll group.
    ///
    /// If there are received messages currently pending on the connection, an attempt
    /// is made to add them to the queue of messages for the poll group in approximately
    /// the order that would have applied if the connection was already part of the poll
    /// group at the time that the messages were received.
    ///
    /// Returns false if the connection handle is invalid, or if the poll group handle
    /// is invalid (and not k_HSteamNetPollGroup_Invalid).
    public bool SetConnectionPollGroup(uint pollGroup, uint connection) {
        return Native.SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup(nativeSockets, connection, pollGroup);
    }

    /// Invoke all callback functions queued for this interface.
    /// See k_ESteamNetworkingConfig_Callback_ConnectionStatusChanged, etc
    ///
    /// You don't need to call this if you are using Steam's callback dispatch
    /// mechanism (SteamAPI_RunCallbacks and SteamGameserver_RunCallbacks).
    public void RunCallbacks() {
        Native.SteamAPI_ISteamNetworkingSockets_RunCallbacks(nativeSockets);
    }

    /// Fetch the next available message(s) from the connection, if any.
    /// Returns the number of messages returned into your array, up to nMaxMessages.
    /// If the connection handle is invalid, -1 is returned.
    ///
    /// The order of the messages returned in the array is relevant.
    /// Reliable messages will be received in the order they were sent (and with the
    /// same sizes --- see SendMessageToConnection for on this subtle difference from a stream socket).
    ///
    /// Unreliable messages may be dropped, or delivered out of order with respect to
    /// each other or with respect to reliable messages.  The same unreliable message
    /// may be received multiple times.
    ///
    /// If any messages are returned, you MUST call SteamNetworkingMessage_t::Release() on each
    /// of them free up resources after you are done.  It is safe to keep the object alive for
    /// a little while (put it into some queue, etc), and you may call Release() from any thread.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReceiveMessagesOnConnection(uint connection, MessageCallback callback, int maxMessages) {
		if (maxMessages > Library.maxMessagesPerBatch)
			throw new ArgumentOutOfRangeException(nameof(maxMessages));

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

    /// Same as ReceiveMessagesOnConnection, but will return the next messages available
    /// on any connection in the poll group.  Examine SteamNetworkingMessage_t::m_conn
    /// to know which connection.  (SteamNetworkingMessage_t::m_nConnUserData might also
    /// be useful.)
    ///
    /// Delivery order of messages among different connections will usually match the
    /// order that the last packet was received which completed the message.  But this
    /// is not a strong guarantee, especially for packets received right as a connection
    /// is being assigned to poll group.
    ///
    /// Delivery order of messages on the same connection is well defined and the
    /// same guarantees are present as mentioned in ReceiveMessagesOnConnection.
    /// (But the messages are not grouped by connection, so they will not necessarily
    /// appear consecutively in the list; they may be interleaved with messages for
    /// other connections.)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ReceiveMessagesOnPollGroup(uint pollGroup, MessageCallback callback, int maxMessages) {
		if (maxMessages > Library.maxMessagesPerBatch)
			throw new ArgumentOutOfRangeException(nameof(maxMessages));

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
}