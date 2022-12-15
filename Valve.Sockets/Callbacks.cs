using Valve.Sockets.Networking;
using Valve.Sockets.Types;

namespace Valve.Sockets;

public delegate void DebugCallback(DebugType type, string message);

public delegate void StatusCallback(ref StatusInfo info);

public delegate void MessageCallback(in Message message);