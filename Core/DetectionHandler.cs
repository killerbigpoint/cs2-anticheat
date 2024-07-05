using CounterStrikeSharp.API.Modules.Utils;
using System.Reflection;
using TBAntiCheat.Detections;
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
                    ACCore.Log($"[TBAC] {metadata.player.Controller.PlayerName} is suspected of using {metadata.detection.Name} ({metadata.reason})");
                    break;
                }

                case ActionType.Kick:
                {
                    ACCore.Log($"[TBAC] {metadata.player.Controller.PlayerName} was kicked for using {metadata.detection.Name} ({metadata.reason})");
                        PlayerUtils.KickPlayer(metadata.player, metadata.detection.Name);

                        break;
                }

                case ActionType.Ban: //made ban to work
                {
                    ACCore.Log($"[TBAC] {metadata.player.Controller.PlayerName} was banned for using {metadata.detection.Name} ({metadata.reason})");
                        PlayerUtils.BanPlayer(metadata.player, metadata.detection.Name);

                    break;
                }
            }
        }

        // Added this for chat notifications. Credits to Gold KingZ
        public static class Chat
        {
            private static readonly Dictionary<string, char> PredefinedColors = typeof(ChatColors)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToDictionary(field => $"{{{field.Name}}}", field => (char)(field.GetValue(null) ?? '\x01'));

            // this applies a color to a string
            public static string ColoredMessage(string message) =>
                PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, $"{color.Value}"));

            // this removes all color tags from a string
            public static string CleanMessage(string message) =>
                PredefinedColors.Aggregate(message, (current, color) => current.Replace(color.Key, "").Replace(color.Value.ToString(), ""));
        }
    }
}
