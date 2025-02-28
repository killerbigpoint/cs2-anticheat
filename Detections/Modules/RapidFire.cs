using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class RapidFireSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Kick;
        public int MaxDetectionsBeforeAction { get; set; } = 5;
    }

    internal class RapidFireData
    {
        internal int lastBulletShotTick;
        internal int rapidDetections;
    }

    /*
     * Module: Rapid Fire
     * Purpose: Detect rapid fire where weapons can fire at speeds that are not normal, either within the same tick or the ticks after
     * NOTE: This is not production ready. It needs a lot of testing so keep that in mind when using this
     */
    internal class RapidFire : BaseDetection
    {
        internal override string Name => "RapidFire";
        internal override ActionType ActionType => config.Config.DetectionAction;

        private readonly BaseConfig<RapidFireSaveData> config;
        private readonly RapidFireData[] playerData;

        internal RapidFire() : base()
        {
            config = new BaseConfig<RapidFireSaveData>("RapidFire");
            playerData = new RapidFireData[Server.MaxPlayers];

            CommandHandler.RegisterCommand("tbac_rapidfire_enable", "Deactivates/Activates RapidFire detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_rapidfire_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);

            Globals.Log($"[TBAC] RapidFire Initialized");
        }

        internal override void OnPlayerJoin(PlayerData player)
        {
            playerData[player.Index] = new RapidFireData()
            {
                lastBulletShotTick = 0,
                rapidDetections = 0
            };
        }

        internal override void OnPlayerShoot(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            CPlayer_WeaponServices? weaponServices = player.Pawn.WeaponServices;
            if (weaponServices == null)
            {
                return;
            }

            CBasePlayerWeapon? weaponBase = weaponServices.ActiveWeapon.Value;
            if (weaponBase == null)
            {
                return;
            }

            CCSWeaponBaseGun weapon = new CCSWeaponBaseGun(weaponBase.Handle);
            if (weaponBase.DesignerName == "weapon_healthshot")
            {
                return;
            }

            int nextAttack;
            switch (weapon.WeaponMode)
            {
                case CSWeaponMode.Primary_Mode: nextAttack = weapon.NextPrimaryAttackTick; break;
                case CSWeaponMode.Secondary_Mode: nextAttack = weapon.NextSecondaryAttackTick; break;
                default: Globals.Log($"[TBAC] Unsupported WeaponMode: {(int)weapon.WeaponMode} on {weaponBase.DesignerName}"); return;
            }

            int serverTickCount = Server.TickCount;
            RapidFireData data = playerData[player.Index];

            int tickDiff = serverTickCount - data.lastBulletShotTick;
            if (tickDiff > 32)
            {
                data.lastBulletShotTick = serverTickCount;
                return;
            }

            int nextAttackDiff = serverTickCount - nextAttack;
            if (tickDiff == 1)
            {
                string detection = $"RapidFire Detected -> Player: {player.Controller.PlayerName} | Weapon: {weaponBase.DesignerName} | ServerTickCount: {serverTickCount} | TickDiff: {tickDiff} | NextAttackDiff: {nextAttackDiff} | NextAttack: {nextAttack} | LastBulletShotTick: {data.lastBulletShotTick}";
                Globals.Log($"[TBAC] {detection}");

                data.rapidDetections++;
            }
            else if (nextAttackDiff > 32)
            {
                string detection = $"RapidFire Detected -> Player: {player.Controller.PlayerName} | Weapon: {weaponBase.DesignerName} | ServerTickCount: {serverTickCount} | TickDiff: {tickDiff} | NextAttackDiff: {nextAttackDiff} | NextAttack: {nextAttack} | LastBulletShotTick: {data.lastBulletShotTick}";
                Globals.Log($"[TBAC] {detection}");

                data.rapidDetections++;
            }

            if (data.rapidDetections >= config.Config.MaxDetectionsBeforeAction)
            {
                string reason = $"RapidFire -> Player: {player.Controller.PlayerName} | Weapon: {weaponBase.DesignerName} | ServerTickCount: {serverTickCount} | TickDiff: {tickDiff} | NextAttackDiff: {nextAttackDiff} | NextAttack: {nextAttack} | LastBulletShotTick: {data.lastBulletShotTick}";
                OnPlayerDetected(player, reason);

                data.rapidDetections = 0;
            }

            data.lastBulletShotTick = serverTickCount;
        }

        // ----- Commands ----- \\

        [RequiresPermissions("@css/admin")]
        private void OnEnableCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (bool.TryParse(arg, out bool state) == false)
            {
                return;
            }

            config.Config.DetectionEnabled = state;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnActionCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (int.TryParse(arg, out int action) == false)
            {
                return;
            }

            ActionType actionType = (ActionType)action;
            if (config.Config.DetectionAction.HasFlag(actionType) == false)
            {
                return;
            }

            config.Config.DetectionAction = actionType;
            config.Save();
        }
    }
}
