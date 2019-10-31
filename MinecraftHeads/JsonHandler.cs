using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftHeads.Responses;

namespace MinecraftHeads
{
    class JsonHandler
    {
        public object DeserializeJsonString(string json)
        {
            return JsonConvert.DeserializeObject<Login>(json);
        }

        public string SerializeJsonString(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject);
        }
    }
}
