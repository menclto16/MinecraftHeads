using System;
using System.Collections.Generic;
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
                GetImage(uuid);
            }
        }

        public async Task GetImage(string uuid)
        {
            var request = await client.GetAsync(headImgApi + uuid);


            if (request.StatusCode == HttpStatusCode.OK)
            {
                var response = await request.Content.ReadAsStringAsync();
                dynamic values = jsonHandler.DeserializeJsonString(response);
            }
        }
    }
}
