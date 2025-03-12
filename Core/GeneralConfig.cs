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
            config = new BaseConfig<GeneralConfigData>("General");
        }

        public static BaseConfig<GeneralConfigData> GetGeneralConfig()
        {
            return config;
        }
    }
}
