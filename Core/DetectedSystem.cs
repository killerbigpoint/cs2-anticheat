using TBAntiCheat.Detections;
using TBAntiCheat.Utils;

namespace TBAntiCheat.Core
{
    internal struct DetectionMetadata
    {
        internal BaseDetection detection;
        internal PlayerData player;
        internal DateTime time;
    }

    internal class DetectedSystem
    {
        internal static void OnPlayerDetected(DetectionMetadata metadata)
        {
            switch (metadata.detection.ActionType)
            {
                case ActionType.None: { return; }

                case ActionType.Log:
                {
                    ACCore.Log($"[TBAC] {metadata.player.Controller.PlayerName} is suspected of using {metadata.detection.Name}");
                    break;
                }

                case ActionType.Kick:
                {
                    ACCore.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.detection.Name}");
                    PlayerUtils.KickPlayer(metadata.player.Controller, $"Kicked for usage of {metadata.detection.Name}");
                    break;
                }

                case ActionType.Ban:
                {
                    break;
                }
            }
        }
    }
}
