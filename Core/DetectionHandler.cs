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
        internal static void OnPlayerDetected(DetectionMetadata metadata)
        {
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
    }
}
