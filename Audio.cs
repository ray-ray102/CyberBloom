using System;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace CyberBloom
{
    public class Audio
    {
        private MediaPlayer? player;

        public void PlayAudioGreeting(Action onAudioComplete)
        {
            try
            {
                // Extract the embedded resource to a temp file
                string tempPath = Path.Combine(Path.GetTempPath(), "cyberbloom_welcome.wav");

                using (Stream? resourceStream = Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream("CyberBloom.welcome.wav"))
                {
                    if (resourceStream != null)
                    {
                        using (FileStream fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Embedded resource not found.");
                        return;
                    }
                }

                // Play from the temp file path
                player = new MediaPlayer();
                player.MediaEnded += (s, e) => onAudioComplete?.Invoke();
                player.Open(new Uri(tempPath, UriKind.Absolute));
                player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error: " + ex.Message);
            }
        }
    }
}