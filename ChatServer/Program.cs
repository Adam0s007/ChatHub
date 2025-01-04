using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using ChatServer.Net.IO;

namespace ChatServer 
{
    public class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the connection to everyone on the server */
                BroadcastConnection();
            }
           
        }

        static void BroadcastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users) // used to broadcast the connection of all users (new user wants to see all the users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode((byte)OpCode.USER_CONNECTED);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode((byte)OpCode.BROADCAST_MESSAGE);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }


        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode((byte)OpCode.DISCONNECT); // opcode 10 for disconnect user
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            BroadcastMessage($"{disconnectedUser.Username} has disconnected");
        }


        public static void PrivateMessage(string senderName, string senderUID, string targetUID, string message)
        {
         
            var targetUser = _users.FirstOrDefault(u => u.UID.ToString() == targetUID);
            if (targetUser != null)
            {   
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode((byte)OpCode.PRIVATE_MESSAGE);
                msgPacket.WriteMessage($"[PM from {senderName}]: {message}");
                targetUser.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

    }

}