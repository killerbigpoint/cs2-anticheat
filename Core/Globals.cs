using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using TBAntiCheat.Detections;
using TBAntiCheat.Detections.Modules;

namespace TBAntiCheat.Core
{
    internal class PlayerData
    {
        internal required CCSPlayerController Controller;
        internal required CCSPlayerPawn Pawn;

        internal required int Index;
        internal required bool IsBot;
    }

    internal static class Globals
    {
        private static bool initializedOnce = false;

        private static ACCore? pluginCore = null;
        private static ILogger? logger = null;

        internal static PlayerData[] Players = [];
        internal static BaseDetection[] Detections = [];

        internal static void PreInit(ACCore core, ILogger log)
        {
            pluginCore = core;
            logger = log;
        }

        internal static void Initialize(bool forceReinitialize)
        {
            Players = new PlayerData[Server.MaxPlayers];
            if (initializedOnce == true && forceReinitialize == false)
            {
                return;
            }

            Log($"[TBAC] Globals Initializing (forced: {forceReinitialize})");

            Detections =
            [
                new Aimbot(),
                //new Backtrack(),
                new BunnyHop(),
                new RapidFire(),
                new UntrustedAngles()
            ];

            initializedOnce = true;

            Log($"[TBAC] Globals Initialized");
        }

        internal static string GetModuleDirectory()
        {
            if (pluginCore == null)
            {
                return string.Empty;
            }

            return pluginCore.ModuleDirectory;
        }

        internal static void Log(string message)
        {
            logger?.Log(LogLevel.Information, message);
        }
    }
}
