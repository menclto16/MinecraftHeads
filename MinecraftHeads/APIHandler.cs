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
using System.Windows.Media.Imaging;
using MinecraftHeads.Responses;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;

namespace MinecraftHeads
{
    public class APIHandler
    {
        public bool LoggedIn = false;

        private static readonly HttpClient client = new HttpClient();
        private JsonHandler jsonHandler = new JsonHandler();
        private FileHandler fileHandler = new FileHandler();
        private DrawingHandler drawingHandler = new DrawingHandler();
        private Login loginData;

        public APIHandler()
        {
            loginData = fileHandler.GetLogin();
            validateLogin();
        }
        private void validateLogin()
        {
            if (loginData != null)
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
        public void GetProperties()
        {
            var request = HttpWebRequest.Create("https://sessionserver.mojang.com/session/minecraft/profile/" + loginData.selectedProfile.id);
            request.Method = "GET";

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                dynamic property = JsonConvert.DeserializeObject(responseString);
                string base64String = property.properties[0].value;
                var base64EncodedBytes = System.Convert.FromBase64String(base64String);
                string value = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                loginData.selectedProfile.properties = JsonConvert.DeserializeObject<ProfileProperties>(value);
                GetSkin();
            }
            catch (WebException e)
            {
                //return e.ToString();
            }
        }

        public BitmapImage GetSkin()
        {
            var request = HttpWebRequest.Create(loginData.selectedProfile.properties.textures.SKIN.url);
            request.Method = "GET";

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                Stream response_stream = response.GetResponseStream();
                Bitmap bitmap = new Bitmap(response_stream);
                return drawingHandler.ConvertImage(bitmap);
                //bitmap.Save("cached_skin.png", ImageFormat.Png);
            }
            catch (WebException e)
            {
                return null;
                //return e.ToString();
            }
        }

        public string Login(string login, string password)
        {
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.username = login;
            loginRequest.password = password;
            loginRequest.requestUser = true;
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
                
                loginData = (Login)jsonHandler.DeserializeJsonString(responseString);
                GetProperties();
                fileHandler.SaveLogin(JsonConvert.SerializeObject(loginData));
                return responseString;
            }
            catch (WebException e)
            {
                //return e.ToString();
                return null;
            }
        }

        public string Validate()
        {
            string requestString = JsonConvert.SerializeObject(loginData);

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
            string requestString = JsonConvert.SerializeObject(loginData);

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
            string requestString = JsonConvert.SerializeObject(loginData);

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

        public List<QuestionResponse> GetQuestions()
        {
            var request = HttpWebRequest.Create("https://api.mojang.com/user/security/challenges");
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + loginData.accessToken;

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                List<QuestionResponse> questions = JsonConvert.DeserializeObject<List<QuestionResponse>>(responseString);
                return questions;
            }
            catch (WebException e)
            {
                //return e.ToString();
                return null;
            }
        }
        public string SendQuestions(List<Answer> answers)
        {
            string requestString = JsonConvert.SerializeObject(answers);
            var byteData = Encoding.UTF8.GetBytes(requestString);

            var request = HttpWebRequest.Create("https://api.mojang.com/user/security/location");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + loginData.accessToken;

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

        public bool IsSecure()
        {
            var request = HttpWebRequest.Create("https://api.mojang.com/user/security/location");
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + loginData.accessToken;

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                if (responseString == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (WebException e)
            {
                //return e.ToString();
                return false;
            }
        }
        /*
        public async Task<Image> ChangeSkin(FileInfo skinPath)
        {
            Response skinUpload = await new UploadSkin(auth.AccessToken, auth.SelectedProfile.Value, skinPath).PerformRequestAsync();
            return await GetImage();
        }
        */
    }
}
