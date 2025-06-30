using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.UserMessages;
using TBAntiCheat.Core;

namespace TBAntiCheat.Handlers
{
    internal static class UserMessagesHandler
    {
        internal static void Initialize(BasePlugin plugin)
        {
            Globals.Log($"[TBAC] UserMessagesHandler Initialized");

            //plugin.HookUserMessage(452, CMsgTEFireBullets, HookMode.Pre);
        }

        private static HookResult CMsgTEFireBullets(UserMessage msg)
        {
            string angles = msg.ReadString("angles");
            int player = msg.ReadInt("player");

            Server.PrintToChatAll($"Player -> {player} | Angles -> {angles}");

            return HookResult.Continue;
        }
    }
}
