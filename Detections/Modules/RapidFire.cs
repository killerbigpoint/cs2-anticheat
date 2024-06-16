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
        public ActionType DetectionAction { get; set; } = ActionType.Log;
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

        internal RapidFire() : base()
        {
            config = new BaseConfig<RapidFireSaveData>("RapidFire");

            CommandHandler.RegisterCommand("tbac_rapidfire_enable", "Deactivates/Activates RapidFire detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_rapidfire_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
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

            CCSWeaponBaseGun? weapon = weaponServices.ActiveWeapon.Value as CCSWeaponBaseGun;
            if (weapon == null)
            {
                return;
            }

            int serverTickCount = Server.TickCount;
            switch (weapon.WeaponMode)
            {
                case CSWeaponMode.Primary_Mode:
                {
                    Server.PrintToChatAll($"{player.Controller.PlayerName} | ServerTick: {serverTickCount} | Primary: {weapon.NextPrimaryAttackTick}");
                    /*if (serverTickCount < weapon.NextPrimaryAttackTick)
                    {
                        string reason = $"RapidFire ->  {weapon.NextSecondaryAttackTick - serverTickCount} tick cooldown | Primary Fire";
                        OnPlayerDetected(player, reason);
                    }*/

                    break;
                }

                case CSWeaponMode.Secondary_Mode:
                {
                    Server.PrintToChatAll($"{player.Controller.PlayerName} | ServerTick: {serverTickCount} | Secondary: {weapon.NextPrimaryAttackTick}");
                    /*if (serverTickCount < weapon.NextSecondaryAttackTick)
                    {
                        string reason = $"RapidFire ->  {weapon.NextSecondaryAttackTick - serverTickCount} tick cooldown | Secondary Fire";
                        OnPlayerDetected(player, reason);
                    }*/

                    break;
                }

                default: //Unsupported type which doesn't exist?
                {
                    break;
                }
            }
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
