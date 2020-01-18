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
        public void UpdatePage()
        {
            ShowSkin();
            loadSavedSkins();
            if (!App.APIHandlerObject.IsSecure()) ShowQuestions();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.APIHandlerObject.Invalidate() == "")
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(App.LoginPageObject);
            }
        }
        public void ShowSkin()
        {
            SkinImage.Source = App.APIHandlerObject.GetSkin(null);
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
                Tile tile = new Tile();
                BitmapImage bitmapImage = new DrawingHandler().ConvertImage(savedSkin.Bitmap);
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Source = bitmapImage;
                image.Width = 55;
                RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
                Label label = new Label();
                label.Content = savedSkin.SkinName;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                StackPanel stackPanel = new StackPanel();
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(label);
                tile.Content = stackPanel;
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
                UpdatePage();
            }
        }
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            dynamic profileProperties = App.APIHandlerObject.GetProperties(SearchBox.Text);
            ProfileName.Content = profileProperties.profileName;
            ProfileUuid.Content = profileProperties.profileId;
            SearchSkinImage.Source = App.APIHandlerObject.GetSkin(SearchBox.Text);
            SaveSkinButton.Visibility = Visibility.Visible;
        }

        private void ShowSaveSkinWindow(object sender, RoutedEventArgs e)
        {
            SaveSkinWindow.IsOpen = true;
        }

        private void SaveSkin(object sender, RoutedEventArgs e)
        {
            new FileHandler().SaveSkin(ProfileUuid.Content.ToString(), SkinNameTextBox.Text);
            SaveSkinWindow.IsOpen = false;
            SkinNameTextBox.Text = "";
            UpdatePage();
        }
    }
}
