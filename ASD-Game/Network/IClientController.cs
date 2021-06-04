﻿using ASD_project.Network.Enum;

namespace ASD_project.Network
{
    public interface IClientController
    {
        public string SessionId { get; }
        public bool IsBackupHost { get; set; }
        public void SendPayload(string payload, PacketType packetType);
        public void SubscribeToPacketType(IPacketHandler packetHandler, PacketType packetType);
        public void SetSessionId(string sessionId);
        public void CreateHostController();
        public string GetOriginId();
        public bool IsHost();
    }
}
