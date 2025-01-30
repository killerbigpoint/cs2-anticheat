using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using TBAntiCheat.Core;

namespace TBAntiCheat.Handlers
{
    internal class VirtualFuncs
    {
        internal static void Initialize()
        {
            VirtualFunctions.CBaseEntity_TakeDamageOld += Bruh;

            ACCore.Log($"[TBAC] VirtualFuncs Initialized");
        }

        private static void Bruh(CEntityInstance entity, CTakeDamageInfo info)
        {
            Server.PrintToChatAll($"Bruh -> {info.GetHitGroup()}");
        }
    }
}
