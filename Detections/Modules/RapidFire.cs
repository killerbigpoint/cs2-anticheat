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
        private readonly Dictionary<int, uint> playerLastWeapon;

        internal RapidFire() : base()
        {
            config = new BaseConfig<RapidFireSaveData>("RapidFire");
            playerLastWeapon = new Dictionary<int, uint>(Server.MaxPlayers);

            CommandHandler.RegisterCommand("tbac_rapidfire_enable", "Deactivates/Activates RapidFire detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_rapidfire_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
        }

        internal override void OnPlayerJoin(PlayerData player)
        {
            playerLastWeapon[player.Index] = 0;
        }

        internal override void OnPlayerShoot(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            if (player.Pawn.WeaponServices == null)
            {
                return;
            }

            CBasePlayerWeapon? weapon = player.Pawn.WeaponServices.ActiveWeapon.Value;
            if (weapon == null)
            {
                return;
            }

            if (playerLastWeapon[player.Index] != weapon.SubclassID.Value)
            {
                playerLastWeapon[player.Index] = weapon.SubclassID.Value;
            }
            else
            {
                int serverTickCount = Server.TickCount;

                bool isSecondaryFire = weapon.NextPrimaryAttackTick != weapon.NextSecondaryAttackTick;
                if (isSecondaryFire == true)
                {
                    if (serverTickCount < weapon.NextSecondaryAttackTick)
                    {
                        string reason = $"RapidFire ->  {weapon.NextSecondaryAttackTick - serverTickCount} tick cooldown | Secondary Fire";
                        OnPlayerDetected(player, reason);
                    }
                }
                else
                {
                    if (serverTickCount < weapon.NextPrimaryAttackTick)
                    {
                        string reason = $"RapidFire ->  {weapon.NextSecondaryAttackTick - serverTickCount} tick cooldown | Primary Fire";
                        OnPlayerDetected(player, reason);
                    }
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
