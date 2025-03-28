using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class BunnyHopSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Log;
        public bool AlertDiscord { get; set; } = false;
    }

    internal class BunnyHopData
    {
        internal int perfectBhops;
    }

    /*
     * Module: Bunny Hop
     * Purpose: Detect players that does tick perfect bunny hops over and over again
     * NOTE: Not production ready. Needs testing
     */
    internal class BunnyHop : BaseModule
    {
        internal override string Name => "BunnyHop";
        internal override ActionType ActionType => config.Config.DetectionAction;
        internal override bool AlertDiscord => config.Config.AlertDiscord;

        private readonly BaseConfig<BunnyHopSaveData> config;
        private readonly BunnyHopData[] playerData;

        internal BunnyHop() : base()
        {
            config = new BaseConfig<BunnyHopSaveData>("BunnyHop");
            playerData = new BunnyHopData[Server.MaxPlayers];

            CommandHandler.RegisterCommand("tbac_bhop_enable", "Deactivates/Activates BunnyHop detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_bhop_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);

            Globals.Log($"[TBAC] BunnyHop Initialized");
        }

        internal override void OnPlayerJoin(PlayerData player)
        {
            if (player.IsBot == true)
            {
                return;
            }

            playerData[player.Index] = new BunnyHopData()
            {
                perfectBhops = 0
            };
        }

        /*internal override void OnPlayerJump(PlayerData player)
        {
            //Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped");
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            BunnyHopData data = playerData[player.Index];

            PlayerButtons buttons = player.Controller.Buttons;
            if (buttons.HasFlag(PlayerButtons.Jump) == true)
            {
                //Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped");
            }
        }*/

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
