using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class NoSpreadSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Log;
    }

    /*
     * Module: No-Spread
     * Purpose: Detect players that consistently hit other players in air or while moving through the use of nospread. We will use a probability system here
     */
    internal class NoSpread : BaseDetection
    {
        internal override string Name => "NoSpread";
        internal override ActionType ActionType => config.Config.DetectionAction;

        private readonly BaseConfig<NoSpreadSaveData> config;

        internal NoSpread() : base()
        {
            config = new BaseConfig<NoSpreadSaveData>("NoSpread");

            CommandHandler.RegisterCommand("tbac_nospread_enable", "Deactivates/Activates BunnyHop detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_nospread_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
        }

        // ----- Commands ----- \\

        [RequiresPermissions("@css/root")]
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

        [RequiresPermissions("@css/root")]
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
