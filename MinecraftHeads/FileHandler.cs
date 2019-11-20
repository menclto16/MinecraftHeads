using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MinecraftHeads.Responses;

namespace MinecraftHeads
{
    class FileHandler
    {
        public void SaveFile(string fileName, string stringToSave)
        {
            File.WriteAllText(fileName, stringToSave);
        }

        public Login GetLogin()
        {
            if (File.Exists("login.json"))
            {
                string jsonFromFile = File.ReadAllText("login.json");

                return JsonConvert.DeserializeObject<Login>(jsonFromFile);
            }
            else return null;
        }
        public void ClearLogin()
        {
            if (File.Exists("login.json"))
            {
                File.Delete("login.json");
            }
        }

        public Login DeserializeJsonString(string json)
        {
            return JsonConvert.DeserializeObject<Login>(json);
        }
    }
}
