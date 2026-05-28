using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CyberBloom
{
    public class Chat
    {
        // Delegate to handle chatbot response generation
        public delegate string ResponseDelegate(string message);

        // Variables for flow and memory
        private bool isNameAsked = false;
        private string userName = "";
        private string lastTopic = "";
        private string favouriteTopic = "";

        // Chat history
        private List<string> chatHistory = new List<string>();

        // Random number generator for varying responses
        private Random random = new Random();

        // Dictionary holding the responses for the 8 topics
        private Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>();

        // Dictionary for topic aliases
        private Dictionary<string, string> topicAliases = new Dictionary<string, string>();

        // File path for user memory
        private string userMemoryFile = "users.txt";

        // Delegate instance for handling bot responses
        private ResponseDelegate responseHandler;

        public Chat()
        {
            SetupResponses();
            SetupAliases();
            // Assign the GenerateBotResponse method to the delegate
            responseHandler = GenerateBotResponse;
        }

        public bool IsNameAsked
        {
            get { return isNameAsked; }
            set { isNameAsked = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string LastTopic
        {
            get { return lastTopic; }
            set { lastTopic = value; }
        }

        public string FavouriteTopic
        {
            get { return favouriteTopic; }
            set { favouriteTopic = value; }
        }

        public List<string> ChatHistory
        {
            get { return chatHistory; }
        }

        private void SetupResponses()
        {
            // Adding multiple responses to each of the 8 required topics
            topicResponses["password safety"] = new List<string> {
                "Always use a strong password with at least 12 characters, numbers, and symbols.",
                "Don't reuse passwords across different sites. Consider using a password manager."
            };
            topicResponses["phishing"] = new List<string> {
                "Phishing is when tricksters send fake emails to steal your data. Always check the sender address.",
                "Never click on suspicious links in emails, even if they look like they are from your bank."
            };
            topicResponses["scams"] = new List<string> {
                "If an offer looks too good to be true, it probably is a scam.",
                "Be wary of strange phone calls asking for your personal information."
            };
            topicResponses["privacy"] = new List<string> {
                "Always check the privacy settings on your social media accounts to see who views your data.",
                "Avoid sharing personal info like your home address online."
            };
            topicResponses["safe browsing"] = new List<string> {
                "Look for the padlock symbol and 'https' in the URL before entering sensitive information.",
                "Keep your web browser updated to protect against the latest threats."
            };
            topicResponses["malware"] = new List<string> {
                "Malware is harmful software. Use strong antivirus software to scan your computer regularly.",
                "Avoid downloading attachments from people you don't know to prevent malware infections."
            };
            topicResponses["two-factor authentication"] = new List<string> {
                "Two-factor authentication (2FA) adds an extra step to logging in, making it much harder for hackers.",
                "Always enable 2FA on important accounts like email and banking."
            };
            topicResponses["social engineering"] = new List<string> {
                "Social engineering is a fancy term for tricking people. Be skeptical of urgent requests for money or data.",
                "Hackers might pretend to be IT support to get your password. Never share it."
            };
        }

        private void SetupAliases()
        {
            // Setup aliases for each topic
            string[] passwordAliases = { "password", "passwords", "password safety", "passcode", "login", "login details" };
            foreach (string alias in passwordAliases)
                topicAliases[alias] = "password safety";

            string[] phishingAliases = { "phishing", "phising", "fake email", "suspicious email", "phish" };
            foreach (string alias in phishingAliases)
                topicAliases[alias] = "phishing";

            string[] scamAliases = { "scam", "scams", "fraud", "fake offer", "scamming", "scammed" };
            foreach (string alias in scamAliases)
                topicAliases[alias] = "scams";

            string[] privacyAliases = { "privacy", "private", "personal info", "data protection", "data privacy", "personal data" };
            foreach (string alias in privacyAliases)
                topicAliases[alias] = "privacy";

            string[] browsingAliases = { "safe browsing", "browser", "website", "https", "link", "suspicious link", "web", "browsing" };
            foreach (string alias in browsingAliases)
                topicAliases[alias] = "safe browsing";

            string[] malwareAliases = { "malware", "virus", "viruses", "trojan", "harmful software", "malicious software" };
            foreach (string alias in malwareAliases)
                topicAliases[alias] = "malware";

            string[] twoFactorAliases = { "2fa", "two factor", "two-factor", "authentication", "otp", "2-factor" };
            foreach (string alias in twoFactorAliases)
                topicAliases[alias] = "two-factor authentication";

            string[] socialEngAliases = { "social engineering", "manipulation", "tricked", "impersonation", "tricking", "social engineer" };
            foreach (string alias in socialEngAliases)
                topicAliases[alias] = "social engineering";
        }

        // Find topic from input using aliases
        private string FindTopic(string input)
        {
            string lowerInput = input.ToLower();

            // Check each word in the input against aliases
            string[] words = lowerInput.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (topicAliases.ContainsKey(word))
                    return topicAliases[word];
            }

            // Check for multi-word aliases
            foreach (string alias in topicAliases.Keys)
            {
                if (lowerInput.Contains(alias))
                    return topicAliases[alias];
            }

            return null;
        }

        // Add message to chat history
        public void AddToChatHistory(string message)
        {
            chatHistory.Add(message);
        }

        // Get full chat history as a string
        public string GetChatHistory()
        {
            if (chatHistory.Count == 0)
            {
                return "Chat history is empty.";
            }

            return string.Join("\n", chatHistory);
        }

        // Save user memory to file
        public void SaveUserMemory()
        {
            try
            {
                string data = $"{userName}|{favouriteTopic}";
                File.WriteAllText(userMemoryFile, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving user memory: " + ex.Message);
            }
        }

        // Load user memory from file
        public void LoadUserMemory(string name)
        {
            try
            {
                if (File.Exists(userMemoryFile))
                {
                    string data = File.ReadAllText(userMemoryFile);
                    string[] parts = data.Split('|');

                    if (parts.Length == 2 && parts[0] == name)
                    {
                        userName = parts[0];
                        favouriteTopic = parts[1];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user memory: " + ex.Message);
            }
        }

        // Simple Levenshtein distance for spelling correction
        private int LevenshteinDistance(string s1, string s2)
        {
            s1 = s1.ToLower();
            s2 = s2.ToLower();

            int len1 = s1.Length;
            int len2 = s2.Length;
            int[,] d = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
                d[i, 0] = i;

            for (int j = 0; j <= len2; j++)
                d[0, j] = j;

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[len1, len2];
        }

        // Find topic with spelling tolerance
        private string FindTopicWithSpellingTolerance(string input)
        {
            string bestMatch = null;
            int bestDistance = 3; // Allow up to 3 character differences

            // Check against all aliases
            foreach (string alias in topicAliases.Keys)
            {
                int distance = LevenshteinDistance(input, alias);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMatch = topicAliases[alias];
                }
            }

            return bestMatch;
        }

        // Detect sentiment from input
        private string DetectSentiment(string input)
        {
            string lowerInput = input.ToLower();

            // Split input to check individual words for better matching
            string[] words = lowerInput.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (word == "worried" || word == "concern" || word == "concerned" || word == "scared" || word == "afraid" || word == "worry")
                    return "worried";

                if (word == "angry" || word == "upset" || word == "mad" || word == "furious" || word == "irritated")
                    return "angry";

                if (word == "confused" || word == "unsure" || word == "lost" || word == "baffled")
                    return "confused";

                if (word == "curious" || word == "inquisitive" || word == "wonder" || word == "wondering")
                    return "curious";

                if (word == "sad" || word == "unhappy" || word == "depressed" || word == "down")
                    return "sad";

                if (word == "happy" || word == "great" || word == "good" || word == "wonderful" || word == "awesome" || word == "excellent")
                    return "happy";

                if (word == "frustrated" || word == "stressed" || word == "overwhelmed" || word == "annoyed")
                    return "frustrated";
            }

            return "neutral";
        }

        // Get response based on sentiment
        private string GetSentimentResponse(string sentiment, string topic, string tipText)
        {
            switch (sentiment)
            {
                case "worried":
                    return $"I understand, {topic} can feel stressful. Here is a tip: {tipText}";
                case "angry":
                    return $"I can see why you might feel upset about {topic}. Here is some advice: {tipText}";
                case "confused":
                    return $"It is completely normal to be confused about {topic}. Let me help: {tipText}";
                case "curious":
                    return $"That is great that you want to learn more about {topic}! {tipText}";
                case "sad":
                    return $"I am here to help with {topic}. {tipText}";
                case "happy":
                    return $"That is wonderful! Here is more about {topic}: {tipText}";
                case "frustrated":
                    return $"I understand your frustration with {topic}. Let me help: {tipText}";
                default:
                    return tipText;
            }
        }

        // Handle tip requests
        private string HandleTipRequest(string input)
        {
            string lowerInput = input.ToLower();

            // Check if user asked for a tip about a specific topic
            string foundTopic = FindTopic(input);
            if (foundTopic != null)
            {
                lastTopic = foundTopic;
                return "Here is a tip about " + foundTopic + ": " + GetRandomResponse(foundTopic);
            }

            // If no topic specified, give a random tip
            List<string> allTopics = topicResponses.Keys.ToList();
            string randomTopic = allTopics[random.Next(allTopics.Count)];
            lastTopic = randomTopic;
            return "Here is a random tip about " + randomTopic + ": " + GetRandomResponse(randomTopic);
        }

        public string GenerateBotResponse(string input)
        {
            string lowerInput = input.ToLower();

            // Check if user wants to see chat history
            if (lowerInput == "history" || lowerInput == "show history")
            {
                return GetChatHistory();
            }

            // Check for tip requests
            if (lowerInput.Contains("give me a tip") || lowerInput.Contains("give me advice") ||
                lowerInput.Contains("help me stay safe") || lowerInput == "tip" || 
                lowerInput.Contains("another tip"))
            {
                return HandleTipRequest(input);
            }

            // Check if user is declaring their favourite topic
            if (lowerInput.Contains("interested in") || lowerInput.Contains("favourite topic is") || lowerInput.Contains("favorite topic is"))
            {
                string foundTopic = FindTopic(input);
                if (foundTopic != null)
                {
                    favouriteTopic = foundTopic;
                    SaveUserMemory();
                    return $"That is great, {userName}! I will remember that you like {foundTopic}. What do you want to know about it?";
                }
            }

            // Detect sentiment
            string sentiment = DetectSentiment(input);

            // Check for follow-up requests ("tell me more")
            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain more") || lowerInput.Contains("more"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && topicResponses.ContainsKey(lastTopic))
                {
                    return GetRandomResponse(lastTopic);
                }
                else
                {
                    return "I am not sure what you want to know more about. Please ask about a specific topic.";
                }
            }

            // Find topic using alias system
            string topic = FindTopic(input);

            if (topic != null)
            {
                lastTopic = topic;
                string baseResponse = GetRandomResponse(topic);

                // Apply sentiment response with topic context
                if (sentiment != "neutral")
                {
                    return GetSentimentResponse(sentiment, topic, baseResponse);
                }
                else if (topic == favouriteTopic)
                {
                    return $"Since {topic} is your favorite topic, {userName}, here is a tip: " + baseResponse;
                }

                return baseResponse;
            }

            // Check for spelling errors (fuzzy matching)
            string[] inputWords = lowerInput.Split(' ');
            foreach (string word in inputWords)
            {
                if (word.Length > 3)
                {
                    string match = FindTopicWithSpellingTolerance(word);
                    if (match != null)
                    {
                        lastTopic = match;
                        string baseResponse = GetRandomResponse(match);

                        // Apply sentiment response
                        if (sentiment != "neutral")
                        {
                            return GetSentimentResponse(sentiment, match, baseResponse);
                        }

                        return baseResponse;
                    }
                }
            }

            // Default fallback response if unknown input
            return $"I am sorry {userName}, I did not understand that. You can ask me about: password safety, phishing, scams, privacy, safe browsing, malware, two-factor authentication, or social engineering.";
        }

        private string GetRandomResponse(string topic)
        {
            List<string> responses = topicResponses[topic];
            int index = random.Next(responses.Count);
            return responses[index];
        }

        // Public method that uses the delegate to handle bot response
        public string ProcessUserInput(string userMessage)
        {
            // Call the delegate to generate the bot response
            return responseHandler(userMessage);
        }
    }
}