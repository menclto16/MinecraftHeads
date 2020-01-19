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
using System.Security;

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
        public dynamic GetProperties(string id)
        {
            if (!isValidGuid(id)) id = GetUuid(id);
            var request = HttpWebRequest.Create("https://sessionserver.mojang.com/session/minecraft/profile/" + id);
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
                fileHandler.SaveFile("cache/profiles/" + id + ".json", value);
                return JsonConvert.DeserializeObject<dynamic>(value);
            }
            catch (WebException e)
            {
                return null;
            }
        }
        public string GetUuid(string name)
        {
            var request = HttpWebRequest.Create("https://api.mojang.com/profiles/minecraft");
            var byteData = Encoding.ASCII.GetBytes('"' + name + '"');
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            try
            {
                var response = (HttpWebResponse)(request.GetResponse());
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                List<dynamic> responseObjectList = JsonConvert.DeserializeObject<List<dynamic>>(responseString);
                return responseObjectList[0].id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public BitmapImage GetSkin(string id)
        {
            if (id == null)
            {
                id = loginData.selectedProfile.id;
            }
            ProfileProperties profileProperties = fileHandler.GetProfile(id);
            if (profileProperties.textures.SKIN == null)
            {
                Bitmap bitmap = new Bitmap("cache/skins/default.png");
                fileHandler.SaveImage("cache/skins/" + id + ".png", bitmap);
                return drawingHandler.ConvertImage(bitmap);
            }
            var request = HttpWebRequest.Create(profileProperties.textures.SKIN.url);
            request.Method = "GET";

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                Stream response_stream = response.GetResponseStream();
                Bitmap bitmap = new Bitmap(response_stream);
                if (id == loginData.selectedProfile.id)
                {
                    fileHandler.SaveImage("cache/skins/current.png", bitmap);
                }
                else
                {
                    fileHandler.SaveImage("cache/skins/" + id + ".png", bitmap);
                }
                
                return drawingHandler.ConvertImage(bitmap);
            }
            catch (WebException e)
            {
                return null;
                //return e.ToString();
            }
        }

        public string UploadSkin(string fileName)
        {
            if (fileName == null)
            {
                fileName = fileHandler.GetSkinPath();
                if (fileName == null) return "Closed";
            }

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.mojang.com/user/profile/" + loginData.selectedProfile.id + "/skin");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "PUT";
            request.Headers["Authorization"] = "Bearer " + loginData.accessToken;
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = request.GetRequestStream();

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string formdataTemplate = "Content-Disposition: form-data; name=\"model\"\r\n\r\n{0}";
            string formitem = string.Format(formdataTemplate, "");
            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
            rs.Write(formitembytes, 0, formitembytes.Length);

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";
            string header = string.Format(headerTemplate, fileName, "image/png");
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = request.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                fileHandler.SaveImage("cache/skins/current.png", new Bitmap(fileName));
                return null;
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                return ex.ToString();
            }
            finally
            {
                request = null;
            }
        }

        public async Task<string> Login(string login, string password)
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

            using (var stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
            }

            try
            {
                var response = (HttpWebResponse)(await request.GetResponseAsync());
                var responseString = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
                
                loginData = (Login)jsonHandler.DeserializeJsonString(responseString);
                GetProperties(loginData.selectedProfile.id);
                fileHandler.SaveFile("login.json", JsonConvert.SerializeObject(loginData, Formatting.Indented));
                App.MainPageObject.UpdateMainPage();
                return responseString;
            }
            catch (Exception ex)
            {
                throw new SecurityException("Bad credentials", ex);
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

                fileHandler.SaveFile("login.json", responseString);
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
                return null;
                //return e.ToString();
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

        private bool isValidGuid(string id)
        {
            Guid guid = new Guid();
            bool isValid = Guid.TryParse(id, out guid);
            return isValid;
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
