using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MojangSharp;
using MojangSharp.Endpoints;
using MojangSharp.Responses;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MinecraftHeads
{
    public class APIHandler
    {
        private HttpClient client = new HttpClient();
        private JsonHandler jsonHandler = new JsonHandler();
        private FileHandler fileHandler = new FileHandler();
        private AuthenticateResponse auth;
        private Login login;

        private string uuidApi = "https://api.minetools.eu/uuid/";
        private string headImgApi = "https://crafatar.com/renders/body/";

        public APIHandler()
        {
            login = fileHandler.GetLogin();
        }

        public async Task<Image> GetSkin(string name)
        {
            var request = await client.GetAsync(uuidApi + name);

            if (request.StatusCode == HttpStatusCode.OK)
            {
                var response = await request.Content.ReadAsStringAsync();
                dynamic values = jsonHandler.DeserializeJsonString(response);
                string uuid = values.id;
                if (uuid != null)
                {
                    return await GetImage(uuid);
                }
            } 
            return new Image() { Source = null};
        }

        public async Task<Image> GetImage(string uuid)
        {
            var request = await client.GetAsync(headImgApi + uuid + "?size=512&default=MHF_Steve&overlay");
            var response = await request.Content.ReadAsStreamAsync();

            using (var stream = new MemoryStream())
            {
                byte[] buffer = new byte[2048];
                int bytesRead;
                while ((bytesRead = response.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                byte[] result = stream.ToArray();
                Image image = new Image();
                image.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return image;
                //File.WriteAllBytes(uuid + ".png", result);
            }
        }

        public async Task<Image> Login(string login, string password)
        {
            auth = await new Authenticate(new Credentials() { Username = login, Password = password }).PerformRequestAsync();
            return await GetImage(auth.SelectedProfile.Value);
        }

        private async void SecureConnection()
        {
            Response response1 = await new Challenges(auth.AccessToken).PerformRequestAsync();
            Response response2 = await new SecureIP(auth.AccessToken).PerformRequestAsync();
        }

        public async Task<Image> ChangeSkin(FileInfo skinPath)
        {
            Response skinUpload = await new UploadSkin(auth.AccessToken, auth.SelectedProfile.Value, skinPath).PerformRequestAsync();
            return await GetImage(auth.SelectedProfile.Value);
        }
    }
}
