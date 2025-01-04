using ChatClient.Net.IO;
using System;
using System.Net.Sockets;
using System.Reflection.Emit;
using OpCode = ChatClient.Net.IO.OpCode;
namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string username) 
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if(!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    // for connection packet we will use opcode 0
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();

                
            }
        }

        public bool IsConnected()
        {
            return _client.Connected;
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch ((OpCode)opcode)
                    {
                        case OpCode.USER_CONNECTED:
                            connectedEvent?.Invoke();
                            break;
                        case OpCode.BROADCAST_MESSAGE:
                            msgReceivedEvent?.Invoke();
                            break;
                        case OpCode.PRIVATE_MESSAGE:
                            msgReceivedEvent?.Invoke();
                            break;
                        case OpCode.DISCONNECT:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("Unknown opcode");
                            break;
                    }
                }
            });

        }
        
        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode((byte)OpCode.BROADCAST_MESSAGE); // 5 - opcode for sending the message
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void SendPrivateMessage(string targetUID, string message)
        {
            if (!_client.Connected) return;

            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode((byte)OpCode.PRIVATE_MESSAGE);
            messagePacket.WriteMessage(targetUID);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

    }
}
