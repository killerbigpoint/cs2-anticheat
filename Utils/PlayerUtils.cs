using CounterStrikeSharp.API;
using TBAntiCheat.Core;

namespace TBAntiCheat.Utils
{
    internal class PlayerUtils
    {
        //Changed command to support SimpleAdmin
        internal static void KickPlayer(PlayerData player, string reason)
        {
            Server.ExecuteCommand($"css_kick #{player.Controller.UserId} {reason}");
        }
        
        //Added BanPlayer using same basic logic as kick
        internal static void BanPlayer(PlayerData player, string reason)
        {
            Server.ExecuteCommand($"css_ban #{player.Controller.UserId} 30 {reason}");
        }
    }
}
