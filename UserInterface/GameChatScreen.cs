﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class GameChatScreen : Screen
    {
        private int _xPosition;
        private int _yPosition;
        private int _width;
        private int _height;
        private Queue<string> messages = new Queue<string>();

        public GameChatScreen(int x, int y, int width, int height)
        {
            _xPosition = x;
            _yPosition = y;
            _width = width;
            _height = height;
        }

        public override void DrawScreen()
        {
            DrawChatBox();          
        }

        private void DrawChatBox()
        {
            DrawBox(_xPosition, _yPosition, _width, _height);
        }

        private void DrawMessages(Queue<string> messageQueue)
        {
            int originalCursorX = Console.CursorLeft;
            int originalCursorY = Console.CursorTop;
            ClearMessages();
            int messageCount = messageQueue.Count;
            for (int i = 0; i < messageCount; i++)
            {
                string message = messageQueue.Peek();
                Console.SetCursorPosition(_xPosition + OFFSET_LEFT, _yPosition + OFFSET_TOP + i);
                Console.Write(message);
                messageQueue.Dequeue();
            }
            Console.SetCursorPosition(originalCursorX, originalCursorY);
        }

        private void ClearMessages()
        {
            for (int i = 0; i <= _height - OFFSET_TOP; i++)
            {
                Console.SetCursorPosition(_xPosition + OFFSET_LEFT, _yPosition + OFFSET_TOP + i);
                Console.Write(new string(' ', _width - BORDER_SIZE));
            }
        }

/*        public void AddMessage(string message)
        {   
            if (message.Length >= _width - BORDER_SIZE)
            {
                int chunkSize = _width - BORDER_SIZE;
                int stringLength = message.Length;
                int maxSize = chunkSize * _height;
                if (stringLength > maxSize)
                {
                    message = message.Substring(0, maxSize - 3) + "...";
                    stringLength = maxSize;
                }

                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength)
                    {
                        chunkSize = stringLength - i;
                    }
                    messages.Enqueue(message.Substring(i, chunkSize));

                    if (messages.Count > _height)
                    {
                        messages.Dequeue();
                    }
                }
            }
            else
            {
                messages.Enqueue(message);
            }
            if (messages.Count > _height)
            {
                messages.Dequeue();
            }
            DrawMessages();
        }*/

        public void ShowMessages(Queue<string> messages)
        {
            Queue<string> messageQueue = new Queue<string>();
            int messageCount = messages.Count;
            for (int i = 0; i < messageCount; i++)
            {
                string message = messages.Dequeue();
                if (message.Length >= _width - BORDER_SIZE)
                {
                    int chunkSize = _width - BORDER_SIZE;
                    int stringLength = message.Length;
                    int maxSize = chunkSize * _height;
                    if (stringLength > maxSize)
                    {
                        message = message.Substring(0, maxSize - 3) + "...";
                        stringLength = maxSize;
                    }

                    for (int j = 0; j < stringLength; j += chunkSize)
                    {
                        if (j + chunkSize > stringLength)
                        {
                            chunkSize = stringLength - j;
                        }
                        messageQueue.Enqueue(message.Substring(j, chunkSize));

                        if (messageQueue.Count > _height)
                        {
                            messageQueue.Dequeue();
                        }
                    }
                }
                else
                {
                    messageQueue.Enqueue(message);
                }
                if (messageQueue.Count > _height)
                {
                    messageQueue.Dequeue();
                }
            }
            DrawMessages(messageQueue);
        }
    }
}
