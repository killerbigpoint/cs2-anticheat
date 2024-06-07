using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TBAntiCheat.Detections;
using TBAntiCheat.Detections.Modules;

namespace TBAntiCheat.Core
{
    internal class PlayerData
    {
        internal required CCSPlayerController Controller;
        internal required CCSPlayerPawn Pawn;

        internal required int Index;
    }

    internal static class Globals
    {
        private static bool InitializedOnce = false;

        internal static Dictionary<uint, PlayerData> Players = null;
        internal static BaseDetection[] Detections = null;

        internal static void Initialize()
        {
            if (InitializedOnce)
            {
                return;
            }

            Players = new Dictionary<uint, PlayerData>(Server.MaxPlayers);
            Detections =
            [
                new Aimbot(),
                new Backtrack(),
                new EyePosition(),
                new EyeAngles()
            ];

            InitializedOnce = true;
            ACCore.Log("[TBAC] Initialized Globals");
        }
    }
}
