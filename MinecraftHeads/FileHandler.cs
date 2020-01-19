using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using MinecraftHeads.Responses;
using System.Drawing;
using Microsoft.Win32;

namespace MinecraftHeads
{
    class FileHandler
    {
        public void SaveFile(string fileName, string stringToSave)
        {
            new FileInfo(fileName).Directory.Create();
            File.WriteAllText(fileName, stringToSave);
        }

        public bool SaveImage(string fileName, Bitmap image)
        {
            if (fileName != "cache/skins/current.png")
            {
                if (File.Exists(fileName)) return false;
            }
            new FileInfo(fileName).Directory.Create();
            image.Save(fileName);
            return true;
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
        public ProfileProperties GetProfile(string id)
        {
            if (File.Exists("cache/profiles/" + id + ".json"))
            {
                string jsonFromFile = File.ReadAllText("cache/profiles/" + id + ".json");

                return JsonConvert.DeserializeObject<ProfileProperties>(jsonFromFile);
            }
            else return null;
        }
        public bool SaveSkin(string id, string name)
        {
            string path = "skinlib/" + name + ".png";
            if (File.Exists(path)) return false;
            new FileInfo(path).Directory.Create();
            File.Copy("cache/skins/" + id + ".png", path);
            return true;
        }
        public void RenameSkin(string oldFileName, string newFileName)
        {
            File.Move(oldFileName, newFileName);
        }

        public void DeleteSkin(string path)
        {
            File.Delete(path);
        }

        public string GetSkinPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) return openFileDialog.FileName;
            return null;
        }

        public List<SavedSkin> GetSavedSkins()
        {
            List<SavedSkin> savedSkins = new List<SavedSkin>();
            string[] fileNames = Directory.GetFiles("skinlib/", "*.png");
            List<Bitmap> images = new List<Bitmap>();
            foreach (var fileName in fileNames)
            {
                SavedSkin savedSkin = new SavedSkin(new Bitmap(fileName), Path.GetFileNameWithoutExtension(fileName));
                savedSkins.Add(savedSkin);
            }
            return savedSkins;
        }
    }
}
