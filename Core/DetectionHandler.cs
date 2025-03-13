using CounterStrikeSharp.API.ValveConstants.Protobuf;
using TBAntiCheat.Detections;
using System.Text.Json;

namespace TBAntiCheat.Core
{
    internal struct DetectionMetadata
    {
        internal BaseModule module;
        internal PlayerData player;
        internal DateTime time;
        internal string reason;
    }

    public class WebhookData
    {
        public string username { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }

    internal static class DetectionHandler
    {
        private static HttpClient webClient = null!;
        private static string webClientAddress = string.Empty;
        private static WebhookData webClientData = null!;

        internal static void OnPlayerDetected(DetectionMetadata metadata)
        {
            if (metadata.module.AlertDiscord == true)
            {
                SendWebhook(metadata);
            }

            switch (metadata.module.ActionType)
            {
                case ActionType.None: { return; }

                case ActionType.Log:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} is suspected of using {metadata.module.Name} ({metadata.reason})");
                    break;
                }

                case ActionType.Kick:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.module.Name} ({metadata.reason})");

                    metadata.player.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_DISCONNECT_BY_SERVER);

                    break;
                }

                case ActionType.Ban:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.module.Name} ({metadata.reason})");

                    BanHandler.BanPlayer(metadata.player, $"Kicked for usage of {metadata.module.Name}");
                    metadata.player.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_DISCONNECT_BY_SERVER);

                    break;
                }
            }
        }

        internal static async void SendWebhook(DetectionMetadata metadata)
        {
            if (webClient == null)
            {
                ulong id = GeneralConfig.GetGeneralConfig().Config.DiscordWebhookID;
                string token = GeneralConfig.GetGeneralConfig().Config.DiscordWebhookToken;

                if (id == 0 || string.IsNullOrEmpty(token) == true)
                {
                    Globals.Log("[TBAC] Can't send webhook message when 'id' or 'token' is not set!");
                    return;
                }

                webClient = new HttpClient();
                webClientAddress = $"https://discordapp.com/api/webhooks/{id}/{token}";

                webClientData = new WebhookData();
                webClientData.username = "TB Anti-Cheat";
            }

            webClientData.content = $"**----- CHEATER DETECTED -----**\n```" +
                $"Detection: {metadata.module.Name}\n" +
                $"Weapon: {metadata.player.GetWeapon().DesignerName}\n" +
                $"Info: {metadata.reason}\n\n" +
                $"Name: {metadata.player.PlayerName}\n" +
                $"SteamID: {metadata.player.SteamID}\n" +
                $"Time: {metadata.time}" +
                $"```";

            string contentJson = JsonSerializer.Serialize(webClientData);
            StringContent content = new StringContent(contentJson, System.Text.Encoding.UTF8, "application/json");

            await webClient.PostAsync(webClientAddress, content);
        }
    }
}
