using CounterStrikeSharp.API.ValveConstants.Protobuf;
using TBAntiCheat.Core;

namespace TBAntiCheat.Utils
{
    internal class PlayerUtils
    {
        internal static void KickPlayer(PlayerData player, string reason)
        {
            player.Controller.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_DISCONNECT_BY_SERVER);
        }
    }
}
