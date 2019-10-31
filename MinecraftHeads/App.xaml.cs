using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftHeads
{
    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static APIHandler APIHandlerObject = new APIHandler();
        public static LoginPage LoginPageObject = new LoginPage();
        public static MainPage MainPageObject = new MainPage();
    }
}
