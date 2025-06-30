using CounterStrikeSharp.API.Modules.Entities;

namespace TBAntiCheat.Core
{
    public class BanHandlerSaveData
    {
        public List<BanMetadata> Bans { get; set; } = new List<BanMetadata>();
    }

    public class BanMetadata
    {
        public required SteamID SteamID { get; set; }
        public required string Reason { get; set; }
        public required string LastKnownUsername { get; set; }
    }

    internal static class BanHandler
    {
        private static BaseConfig<BanHandlerSaveData>? config;

        internal static void Initialize()
        {
            config = new BaseConfig<BanHandlerSaveData>("BannedPlayers");
        }

        internal static void BanPlayer(PlayerData player, string reason)
        {
            SteamID? steamID = player.Controller.AuthorizedSteamID;
            if (steamID == null)
            {
                return;
            }

            if (IsPlayerBanned(steamID) == true)
            {
                return;
            }

            BanMetadata metadata = new BanMetadata()
            {
                SteamID = steamID,
                Reason = reason,
                LastKnownUsername = player.Controller.PlayerName
            };

            if (config == null)
            {
                return;
            }

            config.Config.Bans.Add(metadata);
            config.Save();
        }

        internal static bool IsPlayerBanned(PlayerData player)
        {
            SteamID? steamID = player.Controller.AuthorizedSteamID;
            if (steamID == null)
            {
                return false;
            }

            return IsPlayerBanned(steamID);
        }

        internal static bool IsPlayerBanned(SteamID steamID)
        {
            if (config == null)
            {
                return false;
            }

            List<BanMetadata> banList = config.Config.Bans;
            int banListCount = banList.Count;

            for (int i = 0; i < banListCount; i++)
            {
                if (banList[i].SteamID == steamID)
                {
                    return true;
                }
            }

            return false;
        }

        internal static string GetBanReason(PlayerData player)
        {
            SteamID? steamID = player.Controller.AuthorizedSteamID;
            if (steamID == null)
            {
                return string.Empty;
            }

            return GetBanReason(steamID);
        }

        internal static string GetBanReason(SteamID steamID)
        {
            if (config == null)
            {
                return string.Empty;
            }

            List<BanMetadata> banList = config.Config.Bans;
            int banListCount = banList.Count;

            for (int i = 0; i < banListCount; i++)
            {
                if (banList[i].SteamID == steamID)
                {
                    return banList[i].Reason;
                }
            }

            return string.Empty;
        }
    }
}
