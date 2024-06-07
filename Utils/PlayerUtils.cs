using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace TBAntiCheat.Utils
{
    internal class PlayerUtils
    {
        internal static void KickPlayer(CCSPlayerController player, string reason)
        {
            Server.ExecuteCommand($"kickid {player.UserId} {reason}");
        }

        internal static void BanPlayer(CCSPlayerController player, string reason)
        {
            Server.ExecuteCommand($"kickid {player.UserId} {reason}");

            //TODO: Save this mf and remember for next time
        }

        internal static void NotifyAdmins()
        {

        }
    }
}
