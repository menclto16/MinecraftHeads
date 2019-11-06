using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads.Responses
{
    public class SelectedProfile
    {
        public string id { get; set; }
        public string name { get; set; }
        public string userId { get; set; }
        public long createdAt { get; set; }
        public bool legacyProfile { get; set; }
        public bool suspended { get; set; }
        public bool paid { get; set; }
        public bool migrated { get; set; }
        public bool legacy { get; set; }
    }
    class Login
    {
        public string accessToken { get; set; }
        public string clientToken { get; set; }
        public SelectedProfile selectedProfile { get; set; }
    }
}
