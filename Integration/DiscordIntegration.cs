using TBAntiCheat.Core;
using System.Text.Json;

namespace TBAntiCheat.Integration
{
    internal class DiscordIntegration
    {
        private class WebhookData
        {
#pragma warning disable IDE1006 // Naming Styles
            public string username { get; set; } = string.Empty;
            public string content { get; set; } = string.Empty;
#pragma warning restore IDE1006 // Naming Styles
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
            webClientData.username = "TB Anti-Cheat";

            return true;
        }

        internal static async void SendWebhook(string msg)
        {
            bool result = await SendWebhookAsync(msg);
            if (result == false)
            {
                Globals.Log("[TBAC] Sending webhook failed");
            }
        }

        internal static async Task<bool> SendWebhookAsync(string msg)
        {
            if (webClient == null)
            {
                Globals.Log("[TBAC] Unable to send Webhook (Not initialized)");
                return false;
            }

            webClientData.content = msg;

            // Serialize the data we need for the webhook
            string contentJson;
            try
            {
                contentJson = JsonSerializer.Serialize(webClientData);
            }
            catch (Exception e)
            {
                Globals.Log($"[TBAC] Unable to send Webhook (Serialize Exception | {e.Message})");
                return false;
            }

            // Try sending the response
            HttpResponseMessage response;
            try
            {
                StringContent content = new StringContent(contentJson, System.Text.Encoding.UTF8, "application/json");
                response = await webClient.PostAsync(webClientAddress, content);
            }
            catch (Exception e)
            {
                Globals.Log($"[TBAC] Unable to send Webhook (Request Exception | {e.Message})");
                return false;
            }

            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
                response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                Globals.Log($"----- Discord Error! -----");
                Globals.Log($"Message: {response}");
                Globals.Log($"StackTrace: {Environment.StackTrace}");
            }

            return true;
        }
    }
}
