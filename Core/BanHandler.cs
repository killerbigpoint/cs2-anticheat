using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;

namespace TBAntiCheat.Core
{
    public class BanHandlerSaveData
    {
        public List<BanMetadata> Bans { get; set; } = new List<BanMetadata>();
    }

    public class BanMetadata
    {
        public required ulong SteamID { get; set; }
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
            BanPlayer(player.Controller, reason);
        }

        internal static void BanPlayer(CCSPlayerController controller, string reason)
        {
            SteamID? steamID = controller.AuthorizedSteamID;
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
                SteamID = steamID.SteamId64,
                Reason = reason,
                LastKnownUsername = controller.PlayerName
            };

            if (config == null)
            {
                return;
            }

            config.Config.Bans.Add(metadata);
            config.Save();
        }

        internal static bool UnbanPlayer(string steamIdStr)
        {
            if (config == null)
            {
                return false;
            }

            // Remove player from the ban list by matching SteamID string or ulong
            int removedCount = config.Config.Bans.RemoveAll(b => 
                b.SteamID.ToString() == steamIdStr || 
                steamIdStr.Contains(b.SteamID.ToString()));
            
            if (removedCount > 0)
            {
                config.Save();
                return true;
            }

            return false;
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
            if (steamID == null || config == null)
            {
                return false;
            }

            ulong steamId64 = steamID.SteamId64;
            List<BanMetadata> banList = config.Config.Bans;
            int banListCount = banList.Count;

            for (int i = 0; i < banListCount; i++)
            {
                if (banList[i].SteamID == steamId64)
                {
                    return true;
                }
            }

            return false;
        }

        internal static string GetBanReason(PlayerData player)
        {
            return GetBanReason(player.Controller.AuthorizedSteamID!);
        }

        internal static string GetBanReason(CCSPlayerController controller)
        {
            return GetBanReason(controller.AuthorizedSteamID!);
        }

        internal static string GetBanReason(SteamID steamID)
        {
            if (steamID == null || config == null)
            {
                return string.Empty;
            }

            ulong steamId64 = steamID.SteamId64;
            List<BanMetadata> banList = config.Config.Bans;
            int banListCount = banList.Count;

            for (int i = 0; i < banListCount; i++)
            {
                if (banList[i].SteamID == steamId64)
                {
                    return banList[i].Reason;
                }
            }

            return string.Empty;
        }

        internal static List<BanMetadata> GetBans()
        {
            return config?.Config.Bans ?? new List<BanMetadata>();
        }
    }
}
