﻿using TBAntiCheat.Detections;
using TBAntiCheat.Utils;

namespace TBAntiCheat.Core
{
    internal struct DetectionMetadata
    {
        internal BaseDetection detection;
        internal PlayerData player;
        internal DateTime time;
        internal string reason;
    }

    internal static class DetectionHandler
    {
        internal static void OnPlayerDetected(DetectionMetadata metadata)
        {
            switch (metadata.detection.ActionType)
            {
                case ActionType.None: { return; }

                case ActionType.Log:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} is suspected of using {metadata.detection.Name} ({metadata.reason})");
                    break;
                }

                case ActionType.Kick:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.detection.Name} ({metadata.reason})");
                    PlayerUtils.KickPlayer(metadata.player, $"Kicked for usage of {metadata.detection.Name}");

                    break;
                }

                case ActionType.Ban:
                {
                    Globals.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.detection.Name} ({metadata.reason})");

                    BanHandler.BanPlayer(metadata.player, $"Kicked for usage of {metadata.detection.Name}");
                    PlayerUtils.KickPlayer(metadata.player, $"Kicked for usage of {metadata.detection.Name}");

                    break;
                }
            }
        }
    }
}
