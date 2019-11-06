using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads
{
    public class Agent
    {
        public string name = "Minecraft";
        public int version = 1;
    }
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string clientToken { get; set; }
        public Agent agent = new Agent();
        public bool requestUser = true;
    }
}
