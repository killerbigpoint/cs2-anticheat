using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;

namespace TBAntiCheat.Handlers
{
    internal class VirtualFuncs
    {
        internal static void Initialize()
        {
            //VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnDamageTaken, HookMode.Post);

            ACCore.Log($"[TBAC] VirtualFuncs Initialized");
        }

        private static HookResult OnDamageTaken(DynamicHook hook)
        {
            CEntityInstance entityInfo = hook.GetParam<CEntityInstance>(0);
            CTakeDamageInfo damageInfo = hook.GetParam<CTakeDamageInfo>(1);

            PlayerData victim = Globals.Players[entityInfo.Index];
            PlayerData attacker = Globals.Players[damageInfo.Attacker.Index];

            Vector damagePosEngine = damageInfo.DamagePosition;
            System.Numerics.Vector3 dmgPos = new System.Numerics.Vector3(damagePosEngine.X, damagePosEngine.Y, damagePosEngine.Z);
            System.Numerics.Vector3 eyeAngle = new System.Numerics.Vector3(attacker.Pawn.EyeAngles.X, attacker.Pawn.EyeAngles.Y, attacker.Pawn.EyeAngles.Z);

            Server.PrintToChatAll($"Damage Taken -> {attacker.Controller.PlayerName} -> {victim.Controller.PlayerName} | HitGroup: {damageInfo.GetHitGroup()} | DamagePosition: {dmgPos} | DamageDirection: {damageInfo.DamageDirection} | {eyeAngle}");

            return HookResult.Continue;
        }
    }
}
