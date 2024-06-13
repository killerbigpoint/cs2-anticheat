using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class UntrustedAnglesSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Kick;
    }

    /*
     * Module: Untrusted Angles
     * Purpose: Detect players which use eye angles that are outside the normal limit
     */
    internal class UntrustedAngles : BaseDetection
    {
        internal override string Name => "UntrustedAngles";
        internal override ActionType ActionType => config.Config.DetectionAction;

        private readonly BaseConfig<UntrustedAnglesSaveData> config;

        internal UntrustedAngles() : base()
        {
            config = new BaseConfig<UntrustedAnglesSaveData>("UntrustedAngles");

            CommandHandler.RegisterCommand("tbac_untrustedangles_enable", "Deactivates/Activates UntrustedAngles detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_untrustedangles_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            QAngle eyeAngles = player.Pawn.EyeAngles;
            float pitch = eyeAngles.X;
            float yaw = eyeAngles.Y;
            float roll = eyeAngles.Z;

            if (pitch < -89f || pitch > 89f)
            {
                string reason = $"Untrusted EyeAngles Pitch -> {pitch}";
                OnPlayerDetected(player, reason);

                return;
            }
            else if (yaw < -180f || yaw > 180f)
            {
                string reason = $"Untrusted EyeAngles Yaw -> {yaw}";
                OnPlayerDetected(player, reason);

                return;
            }
            else if (roll < -50f || roll > -50f)
            {
                string reason = $"Untrusted EyeAngles Roll -> {roll}";
                OnPlayerDetected(player, reason);

                return;
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
