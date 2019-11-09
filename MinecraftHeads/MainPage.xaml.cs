﻿using System;
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
            if (!App.APIHandlerObject.IsSecure()) ShowQuestions();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (App.APIHandlerObject.Invalidate() == "")
            {
                ((MainWindow)Application.Current.MainWindow).MainFrame.Navigate(App.LoginPageObject);
            }
        }
        public async void ShowSkin()
        {
            Image image = await App.APIHandlerObject.GetImage();
            SkinImage.Source = image.Source;
        }

        public void ShowQuestions()
        {
            questions = App.APIHandlerObject.GetQuestions();
            Question1.Content = questions[0].question.question;
            Question2.Content = questions[1].question.question;
            Question3.Content = questions[2].question.question;
            QuestionsWindow.IsOpen = true;
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
            App.APIHandlerObject.SendQuestions(answers);
            UpdatePage();
        }
    }
}
