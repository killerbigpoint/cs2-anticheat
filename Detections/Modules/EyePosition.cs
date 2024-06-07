using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class EyePositionSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Log; //Log for now. Consider making Kick once it has been thoroughly tested

        public float MaxOffsetXUpper { get; set; } = 15f;
        public float MaxOffsetXLower { get; set; } = -15f;

        public float MaxOffsetYUpper { get; set; } = 15f;
        public float MaxOffsetYLower { get; set; } = -15f;

        public float MaxOffsetZUpper { get; set; } = 65f;
        public float MaxOffsetZLower { get; set; } = 0f;
    }

    internal class EyePosition : BaseDetection
    {
        internal override string Name => "EyePosition";
        internal override ActionType ActionType => config.Config.DetectionAction;

        private readonly BaseConfig<EyePositionSaveData> config;

        internal EyePosition() : base()
        {
            config = new BaseConfig<EyePositionSaveData>("EyePosition");

            CommandHandler.RegisterCommand("tbac_eyepos_enable", "Deactivates/Activates EyePosition detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_eyepos_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);

            CommandHandler.RegisterCommand("tbac_eyepos_xlower", "Max view offset in lower bounds before action needs to be taken", OnOffsetXLowerCommand);
            CommandHandler.RegisterCommand("tbac_eyepos_xupper", "Max view offset in upper bounds before action needs to be taken", OnOffsetXUpperCommand);

            CommandHandler.RegisterCommand("tbac_eyepos_ylower", "Max view offset in lower bounds before action needs to be taken", OnOffsetYLowerCommand);
            CommandHandler.RegisterCommand("tbac_eyepos_yupper", "Max view offset in upper bounds before action needs to be taken", OnOffsetYUpperCommand);

            CommandHandler.RegisterCommand("tbac_eyepos_zlower", "Max view offset in lower bounds before action needs to be taken", OnOffsetZLowerCommand);
            CommandHandler.RegisterCommand("tbac_eyepos_zupper", "Max view offset in upper bounds before action needs to be taken", OnOffsetZUpperCommand);
        }

        internal override void OnPlayerShoot(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            Vector m_vecViewOffset = Schema.GetDeclaredClass<Vector>(player.Pawn.Handle, "CBaseModelEntity", "m_vecViewOffset");
            float x = m_vecViewOffset.X;
            float y = m_vecViewOffset.Y;
            float z = m_vecViewOffset.Z;

            EyePositionSaveData cfg = config.Config;

            //Normal view offsets. Anything outside of this is abnormal
            if (x > cfg.MaxOffsetXLower && y < cfg.MaxOffsetXUpper &&
                y > cfg.MaxOffsetYLower && y < cfg.MaxOffsetYUpper &&
                z > cfg.MaxOffsetZLower && z < cfg.MaxOffsetZUpper)
            {
                return;
            }

            string reason = $"Abnormal EyePosition -> {x} {y} {z}";
            OnPlayerDetected(player, reason);
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

        //X ViewOffset
        [RequiresPermissions("@css/admin")]
        private void OnOffsetXLowerCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetXLower = offset;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnOffsetXUpperCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetXUpper = offset;
            config.Save();
        }

        //Y ViewOffset
        [RequiresPermissions("@css/admin")]
        private void OnOffsetYLowerCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetYLower = offset;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnOffsetYUpperCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetYUpper = offset;
            config.Save();
        }

        //Z ViewOffset
        [RequiresPermissions("@css/admin")]
        private void OnOffsetZLowerCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetZLower = offset;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnOffsetZUpperCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float offset) == false)
            {
                return;
            }

            config.Config.MaxOffsetZUpper = offset;
            config.Save();
        }
    }
}
