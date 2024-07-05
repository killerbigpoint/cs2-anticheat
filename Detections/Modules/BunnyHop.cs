using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class BunnyHopSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Log;
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
    internal class BunnyHop : BaseDetection
    {
        internal override string Name => "BunnyHop";
        internal override ActionType ActionType => config.Config.DetectionAction;

        private readonly BaseConfig<BunnyHopSaveData> config;
        private readonly Dictionary<int, BunnyHopData> playerData;

        internal BunnyHop() : base()
        {
            config = new BaseConfig<BunnyHopSaveData>("BunnyHop");
            playerData = new Dictionary<int, BunnyHopData>(Server.MaxPlayers);

            CommandHandler.RegisterCommand("tbac_bhop_enable", "Deactivates/Activates BunnyHop detections", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_bhop_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
        }

        internal override void OnPlayerJoin(PlayerData player)
        {
            playerData.Add(player.Index, new BunnyHopData()
            {
                perfectBhops = 0
            });
        }

        internal override void OnPlayerLeave(PlayerData player)
        {
            playerData.Remove(player.Index);
        }

        internal override void OnPlayerShoot(PlayerData player)
        {
            PlayerFlags flags = (PlayerFlags)player.Pawn.Flags;
            bool onGround = flags.HasFlag(PlayerFlags.FL_ONGROUND);

            //Server.PrintToChatAll($"{player.Controller.PlayerName} | FL_ONGROUND -> {onGround}");
        }

        internal override void OnPlayerJump(PlayerData player)
        {
            //Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped Goofy Ass Mf");
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            BunnyHopData data = playerData[player.Index];

            PlayerButtons buttons = player.Controller.Buttons;
            if (buttons.HasFlag(PlayerButtons.Jump) == true)
            {
                //Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped");
            }
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
