﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class Receiver {
        public void checkActionType(ObjectPayloadDTO objectPayloadDTO) 
        {
            switch (objectPayloadDTO.header.actionType) 
            {
                case "chatAction":
                    Console.WriteLine("Case chatAction");
                    processChatAction(objectPayloadDTO.chatAction);
                    break;
                case "moveAction":
                    Console.WriteLine("Case moveAction");
                    //processMoveAction(payload);
                    break;
                case "attackAction":
                    Console.WriteLine("Case attackAction");
                    //processAttackAction(payload);
                    break;
                case "joinAction":
                    Console.WriteLine("Case joinAction");
                    //processJoinAction(payload);
                    break;
                case "sessionUpdateAction":
                    Console.WriteLine("Case sessionUpdateAction");
                    //processSessionUpdateAction(payload);
                    break;
                default:
                    Console.WriteLine("Not a valid actiontype");
                    break;
            }
        }

        public Boolean checkHeader(PayloadHeaderDTO payloadHeaderDTO)
        {
            Console.WriteLine("Checking session with ID: " + payloadHeaderDTO.sessionID);
            return true;
        }

        public void processChatAction(ChatActionDTO chatActionDTO) {
            //Process chat action
        }

        public void processMoveAction(String payload) {
            //Process move action
        }

        public void processAttackAction(String payload) {
            //Process attack action
        }

        public void processJoinAction(String payload) {
            //Process join action
        }

        public void processSessionUpdateAction(String payload) {
            //Process session update action
        }
    }
}
