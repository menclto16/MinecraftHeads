using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace MinecraftHeads
{
    class FileHandler
    {
        public void SaveLogin(Login login)
        {
            string json = JsonConvert.SerializeObject(login);

            File.WriteAllText("login.json", json);
        }

        public Login GetLogin()
        {
            if (File.Exists("login.json"))
            {
                string jsonFromFile = File.ReadAllText("login.json");

                return JsonConvert.DeserializeObject<Login>(jsonFromFile);
            }
            else return new Login();
        }

        public Login DeserializeJsonString(string json)
        {
            return JsonConvert.DeserializeObject<Login>(json);
        }
    }
}
