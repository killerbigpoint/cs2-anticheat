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
        public bool AlertDiscord { get; set; } = false;
    }

    /*
     * Module: Untrusted Angles
     * Purpose: Detect players which use eye angles that are outside the normal limit
     */
    internal class UntrustedAngles : BaseModule
    {
        internal override string Name => "UntrustedAngles";
        internal override ActionType ActionType => config.Config.DetectionAction;
        internal override bool AlertDiscord => config.Config.AlertDiscord;

        //NOTE: We're adding 0.1 extra here due how floating point numbers work in computers. 180f could theoretically still be 180.0002, etc.
        private const float pitchMin = -89.1f;
        private const float pitchMax = 89.1f;

        private const float yawMin = -180.1f;
        private const float yawMax = 180.1f;

        private const float rollMin = -50.1f;
        private const float rollMax = 50.1f;

        private readonly BaseConfig<UntrustedAnglesSaveData> config;

        internal UntrustedAngles() : base()
        {
            config = new BaseConfig<UntrustedAnglesSaveData>("UntrustedAngles");

            CommandHandler.RegisterCommand("tbac_untrustedangles_enable", "Deactivates/Activates UntrustedAngles detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_untrustedangles_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);

            Globals.Log($"[TBAC] UntrustedAngles Initialized");
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            if (player.IsBot == true)
            {
                return;
            }

            QAngle eyeAngles = player.Pawn.EyeAngles;
            float pitch = eyeAngles.X;
            float yaw = eyeAngles.Y;
            float roll = eyeAngles.Z;

            if (pitch < pitchMin || pitch > pitchMax)
            {
                string reason = $"Untrusted EyeAngles Pitch -> {pitch}";
                OnPlayerDetected(player, reason);

                return;
            }
            else if (yaw < yawMin || yaw > yawMax)
            {
                string reason = $"Untrusted EyeAngles Yaw -> {yaw}";
                OnPlayerDetected(player, reason);

                return;
            }
            else if (roll < rollMin || roll > rollMax)
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
