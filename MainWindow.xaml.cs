using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberBloom
{
    public partial class MainWindow : Window
    {
        private Chat chatManager;
        private Audio audioManager;

        public MainWindow()
        {
            InitializeComponent();
            chatManager = new Chat();
            audioManager = new Audio();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            audioManager.PlayAudioGreeting(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    AddMessage("Hello! Welcome to the Cybersecurity Awareness Chatbot. What is your name?", isBot: true);
                    chatManager.IsNameAsked = true;
                });
            });
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            string input = txtInput.Text.Trim();

            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Please type a message before sending.", "Empty Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            chatManager.AddToChatHistory("User: " + input);
            AddMessage(input, isBot: false);
            txtInput.Clear();

            if (chatManager.IsNameAsked && string.IsNullOrEmpty(chatManager.UserName))
            {
                chatManager.UserName = input;
                chatManager.LoadUserMemory(input);

                string greeting = $"Nice to meet you, {chatManager.UserName}! Let's talk about cybersecurity. What do you want to learn about?";

                if (!string.IsNullOrEmpty(chatManager.FavouriteTopic))
                {
                    greeting = $"Welcome back, {chatManager.UserName}! I remember your favourite topic is {chatManager.FavouriteTopic}. What can I help you with today?";
                }

                chatManager.AddToChatHistory("Bot: " + greeting);
                AddMessage(greeting, isBot: true);
                return;
            }

            string response = chatManager.ProcessUserInput(input);
            chatManager.AddToChatHistory("Bot: " + response);
            AddMessage(response, isBot: true);
        }

        private void AddMessage(string message, bool isBot)
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(
                    isBot
                    ? (Color)ColorConverter.ConvertFromString("#F1E4E8")
                    : (Color)ColorConverter.ConvertFromString("#2D2D34")),
                CornerRadius = new CornerRadius(2),
                Padding = new Thickness(10, 7, 10, 7),
                Margin = new Thickness(
                    isBot ? 0 : 60,  // push user bubble away from left(this is for user messages)
                    3,
                    isBot ? 60 : 0,  // push bot bubble away from right(this is for the bot response)
                    3),
                MaxWidth = 300
            };

            TextBlock text = new TextBlock
            {
                Text = message,
                FontSize = 13,
                Foreground = new SolidColorBrush(
                    isBot
                    ? (Color)ColorConverter.ConvertFromString("#2D2D34")
                    : Colors.White),
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = text;

            DockPanel row = new DockPanel
            {
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right,
                Margin = new Thickness(0, 2, 0, 2)
            };

            row.Children.Add(bubble);
            lstChat.Items.Add(row);
            chatScroller.ScrollToEnd();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            lstChat.Items.Clear();
            AddMessage($"Chat cleared. How else can I help you, {chatManager.UserName}?", isBot: true);
        }
    }
}