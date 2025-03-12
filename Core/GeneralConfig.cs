using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Core
{
    public class GeneralConfigData
    {
        public ulong DiscordWebhookID { get; set; } = 0;
        public string DiscordWebhookToken { get; set; } = string.Empty;
    }

    internal static class GeneralConfig
    {
        private static BaseConfig<GeneralConfigData> config = null!;

        public static void Initialize()
        {
            LoadConfig();

            CommandHandler.RegisterCommand("tbac_reload", "Reloads the config", OnReloadCommand);
        }

        public static BaseConfig<GeneralConfigData> GetGeneralConfig()
        {
            return config;
        }

        private static void LoadConfig()
        {
            config = new BaseConfig<GeneralConfigData>("General");

            Globals.Log($"[TBAC] Loaded general config");
        }

        // ----- Commands ----- \\

        [RequiresPermissions("@css/admin")]
        private static void OnReloadCommand(CCSPlayerController? player, CommandInfo command)
        {
            LoadConfig();
        }
    }
}
