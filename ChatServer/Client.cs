using ChatServer.Net.IO;
using ChatServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());


            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client connected with username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch ((OpCode)opcode)
                    {
                        case OpCode.BROADCAST_MESSAGE:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: {Username} says: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}] {Username} says: {msg}");
                            break;
                        case OpCode.PRIVATE_MESSAGE:
                            var targetUid = _packetReader.ReadMessage();
                            var pmContent = _packetReader.ReadMessage();
                            Program.PrivateMessage(Username, UID.ToString(), targetUid, pmContent);
                            break;
                        default:

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
