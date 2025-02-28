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

            Globals.Log($"[TBAC] VirtualFuncs Initialized");
        }

        private static HookResult OnDamageTaken(DynamicHook hook)
        {
            CTakeDamageInfo damageInfo = hook.GetParam<CTakeDamageInfo>(1);

            CBaseEntity? entity = damageInfo.Attacker.Value;
            if (entity == null)
            {
                return HookResult.Continue;
            }

            int entityIndex = (int)entity.Index;
            if (Globals.PlayerReverseLookup.ContainsKey(entityIndex) == false)
            {
                return HookResult.Continue;
            }

            int properIndex = Globals.PlayerReverseLookup[entityIndex];
            PlayerData player = Globals.Players[properIndex];

            if (player == null)
            {
                return HookResult.Continue;
            }

            if (entity.AbsOrigin == null)
            {
                return HookResult.Continue;
            }

            Vector dmgPos = damageInfo.DamagePosition;
            System.Numerics.Vector3 dmgPosition = new System.Numerics.Vector3(dmgPos.X, dmgPos.Y, dmgPos.Z);

            Vector playerPos = entity.AbsOrigin;
            System.Numerics.Vector3 playerPosition = new System.Numerics.Vector3(playerPos.X, playerPos.Y, playerPos.Z);

            System.Numerics.Vector3 direction = dmgPosition - playerPosition;
            System.Numerics.Vector3 directionNormalized = System.Numerics.Vector3.Normalize(direction);

            System.Numerics.Vector3 eyeAngle = new System.Numerics.Vector3(player.Pawn.EyeAngles.X, player.Pawn.EyeAngles.Y, player.Pawn.EyeAngles.Z);

            Server.PrintToChatAll($"Attacker -> {player.Controller.PlayerName} | DamagePosition: {dmgPosition} | PlayerPosition: {playerPosition}");

            return HookResult.Continue;
        }
    }
}
