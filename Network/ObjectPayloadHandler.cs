﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class ObjectPayloadHandler {
        public void checkActionType(ObjectPayloadDTO objectPayloadDTO) 
        {
            switch (objectPayloadDTO.header.actionType)
            {
                case "chatAction":
                    Console.WriteLine("Case chatAction");
                    //Send payload to chatActionComponent
                    break;
                case "moveAction":
                    Console.WriteLine("Case moveAction");
                    //Send payload to moveActionComponent
                    break;
                case "attackAction":
                    Console.WriteLine("Case attackAction");
                    //Send payload to attackActionComponent
                    break;
                case "joinAction":
                    Console.WriteLine("Case joinAction");
                    //Send payload to joinActionComponent
                    break;
                case "sessionUpdateAction":
                    Console.WriteLine("Case sessionUpdateAction");
                    //Send payload to sessionUpdateActionComponent
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
    }
}
