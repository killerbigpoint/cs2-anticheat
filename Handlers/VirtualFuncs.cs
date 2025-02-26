using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using TBAntiCheat.Core;

namespace TBAntiCheat.Handlers
{
    internal class VirtualFuncs
    {
        internal static void Initialize()
        {
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnDamageTaken, HookMode.Pre);

            ACCore.Log($"[TBAC] VirtualFuncs Initialized");
        }

        private static HookResult OnDamageTaken(DynamicHook hook)
        {
            CTakeDamageInfo damageInfo = hook.GetParam<CTakeDamageInfo>(1);
            PlayerData player = Globals.Players[damageInfo.Attacker.Index];

            Server.PrintToChatAll($"Damage Taken -> {damageInfo.GetHitGroup()} | {player.Controller.PlayerName}");

            return HookResult.Continue;
        }
    }
}
