﻿using Network;
using Newtonsoft.Json;
using Session.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network.DTO;

namespace Session
{
    public class SessionHandler : IPacketHandler, ISessionHandler
    {
        private IClientController _clientController;
        private Session _session;
        private Dictionary<string, PacketDTO> _availableSessions = new();

        public SessionHandler(IClientController clientController)
        {
            _clientController = clientController;
            _clientController.SubscribeToPacketType(this, PacketType.Session);
        }

        public void JoinSession(string sessionId)
        {
            if (!_availableSessions.TryGetValue(sessionId, out PacketDTO packetDTO))
            {
                Console.WriteLine("Could not find game!");
            }
            else
            {
                //TODO: MOVE TO EVENT START GAME
                new Thread(() =>
                {
                    SendPing();
                    Thread.Sleep(1000);
                }).Start();
                
                SessionDTO sessionDto = JsonConvert.DeserializeObject<SessionDTO>(packetDTO.HandlerResponse.ResultMessage);
                _session = new Session(sessionDto.Name);
                _session.SessionId = sessionId;
                _clientController.SetSessionId(sessionId);
                Console.WriteLine("Trying to join game with name: " + _session.Name);

                SessionDTO sessionDTO = new SessionDTO(SessionType.RequestToJoinSession);
                sessionDTO.ClientIds = new List<string>();
                sessionDTO.ClientIds.Add(_clientController.GetOriginId());
                sendSessionDTO(sessionDTO);
            }
        }

        public void CreateSession(string sessionName)
        {
            _session = new Session(sessionName);
            _session.GenerateSessionId();
            _session.AddClient(_clientController.GetOriginId());
            _clientController.CreateHostController();
            _clientController.SetSessionId(_session.SessionId);
            Console.Out.WriteLine("Created session with the name: " + _session.Name);
        }

        public void RequestSessions()
        {
            SessionDTO sessionDTO = new SessionDTO(SessionType.RequestSessions);
            sendSessionDTO(sessionDTO);
        }

        private void sendSessionDTO(SessionDTO sessionDTO)
        {
            var payload = JsonConvert.SerializeObject(sessionDTO);
            _clientController.SendPayload(payload, PacketType.Session);
        }

        public HandlerResponseDTO HandlePacket(PacketDTO packet)
        {
            SessionDTO sessionDTO = JsonConvert.DeserializeObject<SessionDTO>(packet.Payload);
            if (packet.Header.Target == "client" || packet.Header.Target == "host")
            {
                switch (sessionDTO.SessionType)
                {
                    case SessionType.RequestSessions:
                        return handleRequestSessions();
                    case SessionType.RequestToJoinSession:
                        if (packet.Header.SessionID == _session?.SessionId)
                        {
                            return addPlayerToSession(packet);
                        }
                        else
                        {
                            return new HandlerResponseDTO(SendAction.Ignore, null);
                        }
                    case SessionType.ReceivedPing:
                        return handlePingRequest();
                    case SessionType.ReceivedPingResponse:
                        handlePingResponse(packet.Payload);
                        return new HandlerResponseDTO(SendAction.Ignore, null);
                }
            }
            else if (packet.Header.Target == _clientController.GetOriginId())
            {
                if (sessionDTO.SessionType == SessionType.RequestSessions)
                {
                    return addRequestedSessions(packet);
                }

                return new HandlerResponseDTO(SendAction.Ignore, null);
            }
            
            return new HandlerResponseDTO(SendAction.Ignore, null);
        }

        private void handlePingResponse(String payload)
        {
            if (payload.Equals("pong"))
            {
                
            }
            else
            {
                // TODO: Host moet overgenomen worden door backup host
                
            }
            
        }

        private HandlerResponseDTO handlePingRequest()
        {
            String pingResponse = "pong";
            return new HandlerResponseDTO(SendAction.ReturnToSender, pingResponse);
        }

        private HandlerResponseDTO handleRequestSessions()
        {
            SessionDTO sessionDTO = new SessionDTO(SessionType.RequestSessionsResponse);
            sessionDTO.Name = _session.Name;
            var jsonObject = JsonConvert.SerializeObject(sessionDTO);
            return new HandlerResponseDTO(SendAction.ReturnToSender, jsonObject);
        }

        private HandlerResponseDTO addRequestedSessions(PacketDTO packet)
        {
            _availableSessions.TryAdd(packet.Header.SessionID, packet);
            SessionDTO sessionDTO = JsonConvert.DeserializeObject<SessionDTO>(packet.HandlerResponse.ResultMessage);
            Console.WriteLine(packet.Header.SessionID + " Name: " + sessionDTO.Name);
            return new HandlerResponseDTO(SendAction.Ignore, null);
        }

        private HandlerResponseDTO addPlayerToSession(PacketDTO packet)
        {
            SessionDTO sessionDTO = JsonConvert.DeserializeObject<SessionDTO>(packet.Payload);

            if (packet.Header.Target == "host")
            {
                Console.WriteLine(sessionDTO.ClientIds[0] + " Has joined your session: ");
                _session.AddClient(sessionDTO.ClientIds[0]);
                sessionDTO.ClientIds = new List<string>();

                foreach (string client in _session.GetAllClients())
                {
                    sessionDTO.ClientIds.Add(client);
                }
                
                
                return new HandlerResponseDTO(SendAction.SendToClients, JsonConvert.SerializeObject(sessionDTO));
            }
            else
            {
                SessionDTO sessionDTOClients = JsonConvert.DeserializeObject<SessionDTO>(packet.HandlerResponse.ResultMessage);
                _session.EmptyClients();

                Console.Out.WriteLine("Players in your session:");
                foreach (string client in sessionDTOClients.ClientIds)
                {
                    _session.AddClient(client);
                    Console.Out.WriteLine(client);
                }

                return new HandlerResponseDTO(SendAction.Ignore, null);
            }
        }

        public void SendPing()
        {
            //if(_clientController.GetOriginId())
            _clientController.SendPayload("ping", PacketType.Session);
        }
    }
}