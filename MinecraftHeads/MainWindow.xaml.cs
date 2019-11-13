﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using MojangSharp.Responses;

namespace MinecraftHeads
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private APIHandler apiHandler = new APIHandler();
        private FileInfo skinPath;

        public MainWindow()
        {
            InitializeComponent();
            checkLogin();
        }

        private async void GetSkin(object sender, RoutedEventArgs e)
        {
            //Image image = await apiHandler.GetSkin(Username.Text);
            //SkinPreview.Source = image.Source;
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
                skinPath = new FileInfo(openFileDialog.FileName);
        }

        private async void ChangeSkin(object sender, RoutedEventArgs e)
        {
            //Image image = await apiHandler.ChangeSkin(skinPath);
            //SkinPreview.Source = image.Source;
        }
        private void checkLogin()
        {
            if (App.APIHandlerObject.LoggedIn)
            {
                MainFrame.Navigate(App.MainPageObject);
                App.MainPageObject.UpdatePage();
            }
            else
            {
                MainFrame.Navigate(App.LoginPageObject);
            }
        }
    }
}
