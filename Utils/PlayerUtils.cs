using CounterStrikeSharp.API;
using TBAntiCheat.Core;

namespace TBAntiCheat.Utils
{
    internal class PlayerUtils
    {
        internal static void KickPlayer(PlayerData player, string reason)
        {
            Server.ExecuteCommand($"kickid {player.Controller.UserId} {reason}");
        }
    }
}
