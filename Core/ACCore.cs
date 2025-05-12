using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core;
using TBAntiCheat.Handlers;
using TBAntiCheat.Integration;

namespace TBAntiCheat.Core
{
    [MinimumApiVersion(264)]
    public class ACCore : BasePlugin
    {
        public override string ModuleName => "TB Anti-Cheat";
        public override string ModuleVersion => "0.4.1";
        public override string ModuleAuthor => "Killer_bigpoint";
        public override string ModuleDescription => "Anti-Cheat for CS2";

        public override void Load(bool hotReload)
        {
            Globals.PreInit(this, Logger);
            Globals.Log($"[TBAC] Loading (hotReload: {hotReload})");

            if (hotReload == true)
            {
                Globals.Initialize(hotReload);
            }

            BanHandler.Initialize();
            CommandHandler.Initialize(this);

            EventListeners.Initialize(this);
            EventHandlers.Initialize(this, hotReload);

            UserMessagesHandler.Initialize();

            GeneralConfig.Initialize();
            DiscordIntegration.InitializeWebhook();
            DiscordIntegration.SendWebhook("Server Started!");

            Globals.Log($"[TBAC] Loaded (v{ModuleVersion})");
        }
    }
}
