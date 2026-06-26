using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CyberBloom
{
    public partial class MainWindow : Window
    {
        private readonly Chat chatManager;
        private readonly Audio audioManager;
        private readonly ObservableCollection<TaskItem> taskItems = new();
        private readonly List<QuizQuestion> quizQuestions = new()
        {
            new QuizQuestion(
                "Which password is the strongest?",
                new[] { "password123", "CyberBloom!94Shield", "qwerty", "myname" },
                1,
                "A strong password is long and uses a mix of characters."),
            new QuizQuestion(
                "What should you do with a suspicious email link?",
                new[] { "Click it quickly", "Forward it to everyone", "Check the sender and avoid the link", "Reply with your password" },
                2,
                "Check the sender carefully and do not open suspicious links."),
            new QuizQuestion(
                "What does two-factor authentication add?",
                new[] { "A second verification step", "A public password", "A slower internet connection", "A second username" },
                0,
                "Two-factor authentication requires another form of verification."),
            new QuizQuestion(
                "Which address is safer for entering sensitive information?",
                new[] { "A site using HTTP only", "A site using HTTPS", "Any shortened link", "A pop-up advertisement" },
                1,
                "HTTPS helps protect information sent between you and the website."),
            new QuizQuestion(
                "What is phishing?",
                new[] { "Updating antivirus", "Backing up files", "A trick used to steal information", "Creating a strong password" },
                2,
                "Phishing uses fake messages or websites to steal personal information.")
        };
        private int currentQuizQuestion;
        private int quizScore;

        public MainWindow()
        {
            InitializeComponent();
            chatManager = new Chat();
            audioManager = new Audio();

            dgTasks.ItemsSource = taskItems;

            for (int hour = 0; hour < 24; hour++)
            {
                cmbReminderHour.Items.Add(hour.ToString("00"));
            }

            for (int minute = 0; minute < 60; minute++)
            {
                cmbReminderMinute.Items.Add(minute.ToString("00"));
            }

            cmbReminderHour.SelectedIndex = DateTime.Now.Hour;
            cmbReminderMinute.SelectedIndex = DateTime.Now.Minute;
            LoadQuizQuestion();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            audioManager.PlayAudioGreeting(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    AddMessage("Hello! Welcome to the Cybersecurity Awareness Chatbot. What is your name?", isBot: true);
                    chatManager.IsNameAsked = true;
                });
            });

            await LoadTasksAsync(checkReminders: true);
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
                Margin = new Thickness(isBot ? 0 : 60, 3, isBot ? 60 : 0, 3),
                MaxWidth = 650
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

        private async void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string description = txtTaskDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Task Title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TryGetReminderMinutes(out int? reminderMinutes))
            {
                return;
            }

            DateTime? reminderDateTime = GetReminderDateTime();

            TaskItem task = new TaskItem
            {
                Title = title,
                Description = string.IsNullOrWhiteSpace(description) ? null : description,
                ReminderDateTime = reminderDateTime,
                ReminderMinutes = reminderMinutes,
                IsCompleted = false
            };

            try
            {
                using CyberBloomContext context = new CyberBloomContext();
                context.Tasks.Add(task);
                await context.SaveChangesAsync();

                AddActivity($"Task added: {task.Title}");
                ClearTaskInputs();
                await LoadTasksAsync(checkReminders: false);
            }
            catch (Exception ex)
            {
                ShowDatabaseError("Adding the task", ex);
            }
        }

        private async void MarkCompleted_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is not TaskItem selectedTask)
            {
                MessageBox.Show("Please select a task first.", "Select Task", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using CyberBloomContext context = new CyberBloomContext();
                TaskItem? task = await context.Tasks.FindAsync(selectedTask.TaskID);

                if (task == null)
                {
                    MessageBox.Show("The selected task no longer exists.", "Task Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadTasksAsync(checkReminders: false);
                    return;
                }

                task.IsCompleted = true;
                await context.SaveChangesAsync();

                AddActivity($"Task completed: {task.Title}");
                await LoadTasksAsync(checkReminders: false);
            }
            catch (Exception ex)
            {
                ShowDatabaseError("Completing the task", ex);
            }
        }

        private async void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is not TaskItem selectedTask)
            {
                MessageBox.Show("Please select a task first.", "Select Task", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult answer = MessageBox.Show(
                $"Delete '{selectedTask.Title}'?",
                "Delete Task",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                using CyberBloomContext context = new CyberBloomContext();
                TaskItem? task = await context.Tasks.FindAsync(selectedTask.TaskID);

                if (task != null)
                {
                    context.Tasks.Remove(task);
                    await context.SaveChangesAsync();
                    AddActivity($"Task deleted: {task.Title}");
                }

                await LoadTasksAsync(checkReminders: false);
            }
            catch (Exception ex)
            {
                ShowDatabaseError("Deleting the task", ex);
            }
        }

        private async void RefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            await LoadTasksAsync(checkReminders: false);
        }

        private async Task LoadTasksAsync(bool checkReminders)
        {
            try
            {
                using CyberBloomContext context = new CyberBloomContext();
                var savedTasks = await context.Tasks
                    .OrderBy(task => task.IsCompleted)
                    .ThenBy(task => task.ReminderDateTime)
                    .ThenBy(task => task.TaskID)
                    .ToListAsync();

                taskItems.Clear();
                foreach (TaskItem task in savedTasks)
                {
                    taskItems.Add(task);
                }

                if (checkReminders)
                {
                    CheckReminders();
                }
            }
            catch (Exception ex)
            {
                ShowDatabaseError("Loading tasks", ex);
            }
        }

        private void CheckReminders()
        {
            DateTime now = DateTime.Now;
            var dueTasks = taskItems
                .Where(task => !task.IsCompleted && task.ReminderDateTime.HasValue)
                .Where(task => task.ReminderDateTime!.Value
                    .AddMinutes(-(task.ReminderMinutes ?? 0)) <= now)
                .ToList();

            foreach (TaskItem task in dueTasks)
            {
                AddActivity($"Reminder due: {task.Title}");
            }

            if (dueTasks.Count > 0)
            {
                string taskNames = string.Join(Environment.NewLine, dueTasks.Select(task => "- " + task.Title));
                MessageBox.Show(
                    "These task reminders are due:" + Environment.NewLine + Environment.NewLine + taskNames,
                    "Task Reminders",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private DateTime? GetReminderDateTime()
        {
            if (!dpReminderDate.SelectedDate.HasValue)
            {
                return null;
            }

            int hour = cmbReminderHour.SelectedIndex >= 0 ? cmbReminderHour.SelectedIndex : 0;
            int minute = cmbReminderMinute.SelectedIndex >= 0 ? cmbReminderMinute.SelectedIndex : 0;

            return dpReminderDate.SelectedDate.Value.Date.AddHours(hour).AddMinutes(minute);
        }

        private bool TryGetReminderMinutes(out int? reminderMinutes)
        {
            string text = txtReminderMinutes.Text.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                reminderMinutes = null;
                return true;
            }

            if (int.TryParse(text, out int minutes))
            {
                reminderMinutes = minutes;
                return true;
            }

            reminderMinutes = null;
            MessageBox.Show(
                "Reminder minutes must be a whole number. Use a positive number for minutes before, or a negative number for minutes after.",
                "Reminder Minutes",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }

        private void ClearTaskInputs()
        {
            txtTaskTitle.Clear();
            txtTaskDescription.Clear();
            dpReminderDate.SelectedDate = null;
            txtReminderMinutes.Clear();
            cmbReminderHour.SelectedIndex = DateTime.Now.Hour;
            cmbReminderMinute.SelectedIndex = DateTime.Now.Minute;
            txtTaskTitle.Focus();
        }

        private void AddActivity(string message)
        {
            lstActivityLog.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm} - {message}");
        }

        private void SubmitQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            int selectedAnswer = GetSelectedQuizAnswer();

            if (selectedAnswer == -1)
            {
                MessageBox.Show("Please select an answer.", "Quiz", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            QuizQuestion question = quizQuestions[currentQuizQuestion];
            bool isCorrect = selectedAnswer == question.CorrectAnswerIndex;

            if (isCorrect)
            {
                quizScore++;
                txtQuizFeedback.Text = "Correct. " + question.Explanation;
            }
            else
            {
                txtQuizFeedback.Text = "Incorrect. " + question.Explanation;
            }

            AddActivity($"Quiz question {currentQuizQuestion + 1}: {(isCorrect ? "correct" : "incorrect")}");
            MessageBox.Show(txtQuizFeedback.Text, "Quiz Answer", MessageBoxButton.OK, MessageBoxImage.Information);

            currentQuizQuestion++;
            if (currentQuizQuestion >= quizQuestions.Count)
            {
                MessageBox.Show(
                    $"Quiz complete. Your score is {quizScore} out of {quizQuestions.Count}.",
                    "Quiz Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                AddActivity($"Quiz completed: {quizScore}/{quizQuestions.Count}");
                currentQuizQuestion = 0;
                quizScore = 0;
            }

            LoadQuizQuestion();
        }

        private int GetSelectedQuizAnswer()
        {
            if (rbQuizOption1.IsChecked == true) return 0;
            if (rbQuizOption2.IsChecked == true) return 1;
            if (rbQuizOption3.IsChecked == true) return 2;
            if (rbQuizOption4.IsChecked == true) return 3;
            return -1;
        }

        private void LoadQuizQuestion()
        {
            QuizQuestion question = quizQuestions[currentQuizQuestion];
            txtQuizProgress.Text = $"Question {currentQuizQuestion + 1} of {quizQuestions.Count}   Score: {quizScore}";
            txtQuizQuestion.Text = question.Question;
            rbQuizOption1.Content = question.Options[0];
            rbQuizOption2.Content = question.Options[1];
            rbQuizOption3.Content = question.Options[2];
            rbQuizOption4.Content = question.Options[3];
            rbQuizOption1.IsChecked = false;
            rbQuizOption2.IsChecked = false;
            rbQuizOption3.IsChecked = false;
            rbQuizOption4.IsChecked = false;
            txtQuizFeedback.Text = string.Empty;
        }

        private void ClearActivityLog_Click(object sender, RoutedEventArgs e)
        {
            lstActivityLog.Items.Clear();
        }

        private void ShowDatabaseError(string action, Exception exception)
        {
            string realError = exception.GetBaseException().Message;

            MessageBox.Show(
                $"{action} failed." + Environment.NewLine + Environment.NewLine +
                $"Database error: {realError}" + Environment.NewLine + Environment.NewLine +
                "Make sure a standalone MySQL server is running and the connection string is correct. " +
                "The EF migrations are already included, so run Update-Database in Package Manager Console.",
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        private class QuizQuestion
        {
            public string Question { get; }
            public string[] Options { get; }
            public int CorrectAnswerIndex { get; }
            public string Explanation { get; }

            public QuizQuestion(string question, string[] options, int correctAnswerIndex, string explanation)
            {
                Question = question;
                Options = options;
                CorrectAnswerIndex = correctAnswerIndex;
                Explanation = explanation;
            }
        }
    }
}
