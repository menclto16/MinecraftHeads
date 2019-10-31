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
using MinecraftHeads.Responses;
using Newtonsoft.Json;

namespace MinecraftHeads
{
    public class APIHandler
    {
        public bool LoggedIn = false;

        private static readonly HttpClient client = new HttpClient();
        private JsonHandler jsonHandler = new JsonHandler();
        private FileHandler fileHandler = new FileHandler();
        private AuthenticateResponse auth;
        private Login loginObject;

        private string uuidApi = "https://api.minetools.eu/uuid/";
        private string headImgApi = "https://crafatar.com/renders/body/";

        public APIHandler()
        {
            loginObject = fileHandler.GetLogin();
            validateLogin();
        }
        private void validateLogin()
        {
            if (loginObject != null)
            {
                if (Validate() == "")
                {
                    LoggedIn = true;
                }
                else if (Refresh() != null)
                {
                    if (Validate() == "")
                    {
                        LoggedIn = true;
                    }
                }
            }
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

        public string Login(string login, string password)
        {
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.username = login;
            loginRequest.password = password;
            loginRequest.clientToken = Guid.NewGuid().ToString();

            string requestString = JsonConvert.SerializeObject(loginRequest);
            

            var request = HttpWebRequest.Create("https://authserver.mojang.com/authenticate");
            var byteData = Encoding.ASCII.GetBytes(requestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                fileHandler.SaveLogin(responseString);
                loginObject = (Login)jsonHandler.DeserializeJsonString(responseString);
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }

        public string Validate()
        {
            string requestString = JsonConvert.SerializeObject(loginObject);

            var request = HttpWebRequest.Create("https://authserver.mojang.com/validate");
            var byteData = Encoding.ASCII.GetBytes(requestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }
        public string Refresh()
        {
            string requestString = JsonConvert.SerializeObject(loginObject);

            var request = HttpWebRequest.Create("https://authserver.mojang.com/refresh");
            var byteData = Encoding.ASCII.GetBytes(requestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                fileHandler.SaveLogin(responseString);
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }
        public string Invalidate()
        {
            string requestString = JsonConvert.SerializeObject(loginObject);

            var request = HttpWebRequest.Create("https://authserver.mojang.com/invalidate");
            var byteData = Encoding.ASCII.GetBytes(requestString);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                fileHandler.ClearLogin();
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
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
