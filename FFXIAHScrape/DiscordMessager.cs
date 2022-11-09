using RestSharp;
using System;
using System.Windows.Forms;

namespace FFXIAHScrape.FFXIAHScrape.RareItemAlert
{
    public class DiscordMessager
    {
        public void Post(string message, string webhookUrl)
        {
            try
            {
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddJsonBody(new { Content = message });

                var client = new RestClient(webhookUrl);
                var response = client.Execute(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"We found a watched item, but the discord post returned status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error attempting to send DiscordMessage: {ex.Message}");
            }
        }
    }
}