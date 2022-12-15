using Valve.Sockets.Networking;

namespace Valve.Sockets;

public static class Library {
    public const int maxCloseMessageLength = 128;
    public const int maxErrorMessageLength = 1024;
    public const int maxMessagesPerBatch = 256;
    public const int maxMessageSize = 512 * 1024;
    public const int socketsCallbacks = 1220;

    public static bool Initialize(string errorMessage = null) {
        if (errorMessage != null && errorMessage.Length > maxErrorMessageLength)
            throw new ArgumentOutOfRangeException("Length of the error message must be smaller or equal to " + maxErrorMessageLength);

        return Native.GameNetworkingSockets_Init(IntPtr.Zero, errorMessage);
    }

    public static bool Initialize(ref Identity identity, string errorMessage) {
        if (errorMessage != null && errorMessage.Length > maxErrorMessageLength)
            throw new ArgumentOutOfRangeException("Length of the error message must be smaller or equal to " + maxErrorMessageLength);

        if (Equals(identity, null))
            throw new ArgumentNullException("identity");

        return Native.GameNetworkingSockets_Init(ref identity, errorMessage);
    }

    public static void Deinitialize() {
        Native.GameNetworkingSockets_Kill();
    }
}