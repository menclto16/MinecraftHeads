using System;
using System.Collections.Generic;
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
using MahApps.Metro.SimpleChildWindow;
using MahApps.Metro.Controls;
using MinecraftHeads.Responses;
using System.Drawing;

namespace MinecraftHeads
{
    /// <summary>
    /// Interakční logika pro MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        List<QuestionResponse> questions;

        public MainPage()
        {
            InitializeComponent();
        }
        public void UpdateMainPage()
        {
            ShowSkin();
            if (!App.APIHandlerObject.IsSecure()) ShowQuestions();
        }
        public void UpdateGalleryPage()
        {
            loadSavedSkins();
        }
        private void LogOut(object sender, RoutedEventArgs e)
        {
            if (App.APIHandlerObject.Invalidate() == "")
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(App.LoginPageObject);
            }
        }
        private void ChangeSkin(object sender, RoutedEventArgs e)
        {
            string value = App.APIHandlerObject.UploadSkin(null);
            if (value != null)
            {
                ErrorLabel.Content = "There was an error uploading your skin";
                ErrorDetailTextBlock.Text = value;
                ErrorWindow.IsOpen = true;
            }
            else
            {
                UpdateMainPage();
            }
        }
        public void ShowSkin()
        {
            SkinImage.Source = new DrawingHandler().ConvertImage(new Bitmap("cache/skins/current.png"));
        }

        public void ShowQuestions()
        {
            questions = App.APIHandlerObject.GetQuestions();
            Question1.Content = questions[0].question.question;
            Question2.Content = questions[1].question.question;
            Question3.Content = questions[2].question.question;
            QuestionsWindow.IsOpen = true;
        }

        private void loadSavedSkins()
        {
            SkinWrapPanel.Children.Clear();
            List<SavedSkin> savedSkins = new FileHandler().GetSavedSkins();
            foreach (var savedSkin in savedSkins)
            {
                SkinTile tile = new SkinTile();
                BitmapImage bitmapImage = new DrawingHandler().ConvertImage(savedSkin.Bitmap);
                savedSkin.Bitmap.Dispose();
                tile.SkinImage.Source = bitmapImage;
                tile.SkinName.Content = savedSkin.SkinName;
                SkinWrapPanel.Children.Add(tile);
            }
        }

        private void SendAnswers(object sender, RoutedEventArgs e)
        {
            List<Answer> answers = new List<Answer>();
            answers.Add(new Answer());
            answers.Add(new Answer());
            answers.Add(new Answer());
            answers[0].answer = Answer1.Text;
            answers[0].id = questions[0].answer.id;
            answers[1].answer = Answer2.Text;
            answers[1].id = questions[1].answer.id;
            answers[2].answer = Answer3.Text;
            answers[2].id = questions[2].answer.id;
            if (App.APIHandlerObject.SendQuestions(answers) != null)
            {
                QuestionsWindow.IsOpen = false;
                UpdateMainPage();
            }
        }
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dynamic profileProperties = App.APIHandlerObject.GetProperties(SearchBox.Text);
            if (profileProperties == null)
            {
                ErrorLabel.Content = "Not Found";
                ErrorDetailTextBlock.Text = "User with name/uuid '" + SearchBox.Text + "' was not found...";
                ErrorWindow.IsOpen = true;
            }
            else
            {
                ProfileName.Content = profileProperties.profileName;
                ProfileUuid.Content = profileProperties.profileId;
                SearchSkinImage.Source = App.APIHandlerObject.GetSkin(ProfileUuid.Content.ToString());
                SaveSkinButton.Visibility = Visibility.Visible;
            }
        }

        private void ShowSaveSkinWindow(object sender, RoutedEventArgs e)
        {
            SaveSkinMessage.Content = "";
            SaveSkinWindow.IsOpen = true;
        }

        private void ToggleErrorWindow(object sender, RoutedEventArgs e)
        {
            ErrorWindow.IsOpen = !ErrorWindow.IsOpen;
        }
        private void ToggleSaveSkinWindow(object sender, RoutedEventArgs e)
        {
            SaveSkinWindow.IsOpen = !SaveSkinWindow.IsOpen;
        }
        private void ToggleRenameSkinWindow(object sender, RoutedEventArgs e)
        {
            RenameSkinWindow.IsOpen = !RenameSkinWindow.IsOpen;
        }
        private void SaveSkin(object sender, RoutedEventArgs e)
        {
            if (new FileHandler().SaveSkin(ProfileUuid.Content.ToString(), SkinNameTextBox.Text))
            {
                SaveSkinWindow.IsOpen = false;
                SkinNameTextBox.Text = "";
                UpdateGalleryPage();
            }
            else
            {
                SaveSkinMessage.Content = "Skin name already exists";
            }
        }
        private void AddSkin(object sender, RoutedEventArgs e)
        {
            if (new FileHandler().SaveImage("skinlib/" + AddSkinNameTextBox.Text + ".png", new Bitmap(HiddenSkinPath.Content.ToString())))
            {
                AddSkinWindow.IsOpen = false;
                AddSkinNameTextBox.Text = "";
                UpdateGalleryPage();
            }
            else
            {
                AddSkinMessage.Content = "Skin name already exists";
            }
        }
        private void ShowAddSkinWindow(object sender, RoutedEventArgs e)
        {
            string skinPath = new FileHandler().GetSkinPath();
            if (skinPath != null)
            {
                AddSkinMessage.Content = "";
                HiddenSkinPath.Content = skinPath;
                AddSkinWindow.IsOpen = true;
            }
        }
        public void ShowRenameSkinWindow(string skinName)
        {
            RenameSkinWindow.Title = "Rename '" + skinName + "'";
            RenameSkinOldName.Content = skinName;
            RenameSkinWindow.IsOpen = true;
        }

        private void RenameSkin(object sender, RoutedEventArgs e)
        {
            new FileHandler().RenameSkin("skinlib/" + RenameSkinOldName.Content + ".png", "skinlib/" + SkinRenameNameTextBox.Text + ".png");
            UpdateGalleryPage();
            RenameSkinWindow.IsOpen = false;
        }
    }
}
