﻿using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Core
{
    [MinimumApiVersion(264)]
    public class ACCore : BasePlugin
    {
        public override string ModuleName => "TB Anti-Cheat";
        public override string ModuleVersion => "0.3.5";
        public override string ModuleAuthor => "Killer_bigpoint";
        public override string ModuleDescription => "Anti-Cheat for CS2";

        private static ILogger? logger = null;

        public override void Load(bool hotReload)
        {
            Log($"[TBAC] Loading (hotReload: {hotReload})");

            logger = Logger;

            Globals.PreInit(this);
            if (hotReload == true)
            {
                Globals.Initialize(hotReload);
            }

            BanHandler.Initialize();
            CommandHandler.Initialize(this);

            EventListeners.Initialize(this);
            EventHandlers.Initialize(this, hotReload);

            VirtualFuncs.Initialize();

            Log($"[TBAC] Loaded (v{ModuleVersion})");
        }

        internal static void Log(string message)
        {
            if (logger == null)
            {
                return;
            }

            logger.Log(LogLevel.Information, message);
        }
    }
}
