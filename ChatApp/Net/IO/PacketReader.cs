using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32(); // from BinaryReader class
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length); // save the message in the buffer array starting from offset 0 and read the length of the message

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }


    }
}
