namespace Valve.Sockets.Types.Configuration;

public enum Value {
    Invalid = 0,
    FakePacketLossSend = 2,
    FakePacketLossRecv = 3,
    FakePacketLagSend = 4,
    FakePacketLagRecv = 5,
    FakePacketReorderSend = 6,
    FakePacketReorderRecv = 7,
    FakePacketReorderTime = 8,
    FakePacketDupSend = 26,
    FakePacketDupRecv = 27,
    FakePacketDupTimeMax = 28,
    TimeoutInitial = 24,
    TimeoutConnected = 25,
    SendBufferSize = 9,
    SendRateMin = 10,
    SendRateMax = 11,
    NagleTime = 12,
    IPAllowWithoutAuth = 23,
    MTUPacketSize = 32,
    MTUDataSize = 33,
    Unencrypted = 34,
    EnumerateDevVars = 35,
    SymmetricConnect = 37,
    LocalVirtualPort = 38,
    ConnectionStatusChanged = 201,
    AuthStatusChanged = 202,
    RelayNetworkStatusChanged = 203,
    MessagesSessionRequest = 204,
    MessagesSessionFailed = 205,
    P2PSTUNServerList = 103,
    P2PTransportICEEnable = 104,
    P2PTransportICEPenalty = 105,
    P2PTransportSDRPenalty = 106,
    SDRClientConsecutitivePingTimeoutsFailInitial = 19,
    SDRClientConsecutitivePingTimeoutsFail = 20,
    SDRClientMinPingsBeforePingAccurate = 21,
    SDRClientSingleSocket = 22,
    SDRClientForceRelayCluster = 29,
    SDRClientDebugTicketAddress = 30,
    SDRClientForceProxyAddr = 31,
    SDRClientFakeClusterPing = 36,
    LogLevelAckRTT = 13,
    LogLevelPacketDecode = 14,
    LogLevelMessage = 15,
    LogLevelPacketGaps = 16,
    LogLevelP2PRendezvous = 17,
    LogLevelSDRRelayPings = 18
}
