using TBAntiCheat.Core;
using System.Text.Json;

namespace TBAntiCheat.Integration
{
    internal class DiscordIntegration
    {
        private class WebhookData
        {
            public string Username { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        private static HttpClient webClient = null!;
        private static string webClientAddress = string.Empty;
        private static WebhookData webClientData = null!;

        internal static bool InitializeWebhook()
        {
            ulong id = GeneralConfig.GetGeneralConfig().Config.DiscordWebhookID;
            string token = GeneralConfig.GetGeneralConfig().Config.DiscordWebhookToken;

            if (id == 0 || string.IsNullOrEmpty(token) == true)
            {
                Globals.Log("[TBAC] Unable to initialize Webhook ('id' or 'token' not set)");
                return false;
            }

            webClient = new HttpClient();
            webClientAddress = $"https://discordapp.com/api/webhooks/{id}/{token}";

            webClientData = new WebhookData();
            webClientData.Username = "TB Anti-Cheat";

            return true;
        }

        internal static async void SendWebhook(string msg)
        {
            await SendWebhookAsync(msg);
        }

        internal static async Task<bool> SendWebhookAsync(string msg)
        {
            if (webClient == null)
            {
                Globals.Log("[TBAC] Unable to send Webhook (Not initialized)");
                return false;
            }

            webClientData.Content = msg;

            string contentJson = JsonSerializer.Serialize(webClientData);
            StringContent content = new StringContent(contentJson, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await webClient.PostAsync(webClientAddress, content);

            Globals.Log($"Message -> {response}");
            return true;
        }
    }
}
