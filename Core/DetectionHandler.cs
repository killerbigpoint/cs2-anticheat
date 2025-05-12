using CounterStrikeSharp.API.ValveConstants.Protobuf;
using TBAntiCheat.Detections;
using TBAntiCheat.Integration;

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
        internal static void OnPlayerDetected(DetectionMetadata metadata)
        {
            if (metadata.module.AlertDiscord == true)
            {
                NotifyDiscord(metadata);
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

        private static void NotifyDiscord(DetectionMetadata metadata)
        {
            string content = $"**----- CHEATER DETECTED -----**\n```" +
                $"Detection: {metadata.module.Name}\n" +
                $"Weapon: {metadata.player.GetWeapon().DesignerName}\n" +
                $"Info: {metadata.reason}\n\n" +
                $"Name: {metadata.player.PlayerName}\n" +
                $"SteamID: {metadata.player.SteamID}\n" +
                $"Time: {metadata.time}" +
                $"```";

            DiscordIntegration.SendWebhook(content);
        }
    }
}
