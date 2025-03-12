using Discord.Webhook;
using TBAntiCheat.Detections;
using TBAntiCheat.Utils;

namespace TBAntiCheat.Core
{
    internal struct DetectionMetadata
    {
        internal BaseModule module;
        internal PlayerData player;
        internal DateTime time;
        internal string reason;
    }

    internal static class DetectionHandler
    {
        private static DiscordWebhookClient webClient = null!;

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
                    PlayerUtils.KickPlayer(metadata.player, $"Kicked for usage of {metadata.module.Name}");

                    break;
                }

                case ActionType.Ban:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.module.Name} ({metadata.reason})");

                    BanHandler.BanPlayer(metadata.player, $"Kicked for usage of {metadata.module.Name}");
                    PlayerUtils.KickPlayer(metadata.player, $"Kicked for usage of {metadata.module.Name}");

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

                webClient = new DiscordWebhookClient(id, token);
            }

            string content = $"**----- CHEATER DETECTED -----**\n```" +
                $"Detection: {metadata.module.Name}\n" +
                $"Weapon: {metadata.player.GetWeapon().DesignerName}\n" +
                $"Info: {metadata.reason}\n\n" +
                $"Name: {metadata.player.PlayerName}\n" +
                $"SteamID: {metadata.player.SteamID}\n" +
                $"Time: {metadata.time}" +
                $"```";

            await webClient.SendMessageAsync(content);
        }
    }
}
