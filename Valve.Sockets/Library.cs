using Valve.Sockets.Networking;

namespace Valve.Sockets;

public static class Library {
    public const int maxCloseMessageLength = 128;
    public const int maxErrorMessageLength = 1024;
    public const int maxMessagesPerBatch = 256;
    public const int maxMessageSize = 512 * 1024;
    public const int socketsCallbacks = 1220;


    // Initialize the library.  Optionally, you can set an initial identity for the default
    // interface that is returned by SteamNetworkingSockets().
    //
    // On failure, false is returned, and a non-localized diagnostic message is returned.
    public static bool Initialize(out string errorMessage) {
        return Native.GameNetworkingSockets_Init(IntPtr.Zero, out errorMessage);
    }

    // Initialize the library.  Optionally, you can set an initial identity for the default
    // interface that is returned by SteamNetworkingSockets().
    //
    // On failure, false is returned, and a non-localized diagnostic message is returned.
    public static bool Initialize(Identity identity, out string errorMessage) {
        if (Equals(identity, null))
            throw new ArgumentNullException(nameof(identity));

        return Native.GameNetworkingSockets_Init(identity, out errorMessage);
    }

    // Close all connections and listen sockets and free all resources
    public static void Deinitialize() {
        Native.GameNetworkingSockets_Kill();
    }
}