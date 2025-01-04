using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    public enum OpCode : byte
    {
        CONNECT = 0,
        USER_CONNECTED = 1,
        PRIVATE_MESSAGE = 2,
        BROADCAST_MESSAGE = 5,
        DISCONNECT = 10
    }
}