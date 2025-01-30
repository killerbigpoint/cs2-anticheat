﻿using CounterStrikeSharp.API;
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
        private static bool initializedOnce = false;

        private static ACCore? pluginCore = null;

        internal static Dictionary<uint, PlayerData> Players = [];
        internal static BaseDetection[] Detections = [];

        internal static void PreInit(ACCore core)
        {
            pluginCore = core;
        }

        internal static void Initialize(bool forceReinitialize)
        {
            if (initializedOnce == true && forceReinitialize == false)
            {
                return;
            }

            ACCore.Log($"[TBAC] Globals Initialized -> {forceReinitialize}");

            Players = new Dictionary<uint, PlayerData>(Server.MaxPlayers);
            Detections =
            [
                new Aimbot(),
                //new Backtrack(),
                new BunnyHop(),
                new RapidFire(),
                new UntrustedAngles()
            ];

            initializedOnce = true;
        }

        internal static string GetModuleDirectory()
        {
            if (pluginCore == null)
            {
                return string.Empty;
            }

            return pluginCore.ModuleDirectory;
        }
    }
}
