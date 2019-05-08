using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftHeads
{
    class APIHandler
    {
        HttpClient client = new HttpClient();
        JsonHandler jsonHandler = new JsonHandler();

        private string uuidApi = "https://api.minetools.eu/uuid/";
        private string headImgApi = "https://crafatar.com/avatars/";

        public async Task GetUUID(string name)
        {
            var request = await client.GetAsync(uuidApi + name);
            

            if (request.StatusCode == HttpStatusCode.OK)
            {
                var response = await request.Content.ReadAsStringAsync();
                dynamic values = jsonHandler.DeserializeJsonString(response);
                string uuid = values.id;
                if (uuid != null) await GetImage(uuid);
            }
        }

        public async Task GetImage(string uuid)
        {
            var request = await client.GetAsync(headImgApi + uuid);
            var response = await request.Content.ReadAsStreamAsync();

            using (var stream = new MemoryStream())
            {
                byte[] buffer = new byte[2048]; // read in chunks of 2KB
                int bytesRead;
                while ((bytesRead = response.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                byte[] result = stream.ToArray();
                File.WriteAllBytes(uuid + ".png", result);
            }
        }
    }
}
