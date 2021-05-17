﻿using System;
using Newtonsoft.Json;

namespace Network
{
    public class NetworkComponent : IPacketListener, INetworkComponent
    {
        private WebSocketConnection _webSocketConnection;
        private string _originId;
        private IPacketListener _hostController;
        public IPacketListener HostController { get => _hostController;}
        private IPacketHandler _clientController;
        public IPacketHandler ClientController { get => _clientController;}

        public NetworkComponent()
        {
            _webSocketConnection = new WebSocketConnection(this);
            _originId = Guid.NewGuid().ToString();
        }

        public void ReceivePacket(PacketDTO packet)
        {
            if(_hostController != null)
            {
                if(packet.Header.Target == "host")
                {
                    _hostController.ReceivePacket(packet);
                }
            }
            else if((packet.Header.Target == "client" || packet.Header.Target == _originId) && _clientController != null)
            {
                _clientController.HandlePacket(packet);
            }
        }

        public void SendPacket(PacketDTO packet)
        {
            packet.Header.OriginID = _originId;
            string serializedPacket = JsonConvert.SerializeObject(packet);
            _webSocketConnection.Send(serializedPacket);
        }

        public void SetClientController(IPacketHandler clientController)
        {
            _clientController = clientController;
        }

        public void SetHostController(IPacketListener hostController)
        {
            _hostController = hostController;
        }
        
        public string GetOriginId()
        {
            return _originId;
        }
    }
}
