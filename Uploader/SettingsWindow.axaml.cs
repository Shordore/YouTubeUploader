using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;

namespace Uploader
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void WriteCredentials_Click(object? sender, RoutedEventArgs e)
        {

            string clientId = ClientIdBox.Text;
            string clientSecret = ClientSecretBox.Text;

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                //TODO: Append to console that credentials are empty
                return;
            }

            WriteSecretToFile(clientId, clientSecret);
        }

        private void WriteSecretToFile(string ClientId, string ClientSecret)
        {

            string filePath = "secret.txt";

            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    if (lines.Length >= 2 && ClientId == lines[0] &&
                        ClientSecret == lines[1])
                    {
                        //TODO: Append to console that credentials already exist
                        return;
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {

                //TODO: Append to console that credentials are being written
                writer.WriteLine(ClientId);
                writer.WriteLine(ClientSecret);
            }



        }

    }


}