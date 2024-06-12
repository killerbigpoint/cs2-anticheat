using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Core
{
    [MinimumApiVersion(234)]
    public class ACCore : BasePlugin
    {
        public override string ModuleName => "TB Anti-Cheat";
        public override string ModuleVersion => "0.2.0";
        public override string ModuleAuthor => "Killer_bigpoint";
        public override string ModuleDescription => "Proper Anti-Cheat for CS2";

        private static ACCore core;
        private static ILogger? logger = null;

        public override void Load(bool hotReload)
        {
            core = this;
            logger = Logger;

            BanHandler.InitializeBanHandler();
            CommandHandler.InitializeCommandHandler(this);

            EventListeners.InitializeListeners(this);
            EventHandlers.InitializeHandlers(this);

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

        internal static ACCore GetCore()
        {
            return core;
        }
    }
}
