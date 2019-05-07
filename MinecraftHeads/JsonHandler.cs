using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads
{
    class JsonHandler
    {
        public object DeserializeJsonString(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }
}
