using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Numerics;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TBAntiCheat.Detections.Modules
{
    public class BunnyHopSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Log;
    }

    internal class BunnyHopData
    {
        internal bool lastOnGround;
        internal int lastGroundedTick;
        internal bool lastTickPerfect;

        internal float lastLandTime;

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
                lastOnGround = true,
                lastGroundedTick = 0,
                lastTickPerfect = false,

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

            Server.PrintToChatAll($"{player.Controller.PlayerName} | FL_ONGROUND -> {onGround}");
        }

        internal override void OnPlayerJump(PlayerData player)
        {
            Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped Goofy Ass Mf");
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            BunnyHopData data = playerData[player.Index];

            PlayerButtons buttons = player.Controller.Buttons;
            if (buttons.HasFlag(PlayerButtons.Jump) == true)
            {
                Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped");
            }

            /*PlayerFlags playerFlags = (PlayerFlags)player.Pawn.Flags;
            bool playerOnGround = playerFlags.HasFlag(PlayerFlags.FL_ONGROUND);

            float lastLandTime = player.Pawn.LastLandTime;
            if (data.lastLandTime != lastLandTime)
            {
                Server.PrintToChatAll($"{player.Controller.PlayerName} -> LastLandTime: {lastLandTime}");
                data.lastLandTime = lastLandTime;
            }

            if (data.lastOnGround != playerOnGround)
            {
                if (playerOnGround == true)
                {
                    Server.PrintToChatAll($"{player.Controller.PlayerName} -> Landed");
                }
                else
                {
                    Server.PrintToChatAll($"{player.Controller.PlayerName} -> Jumped");
                }

                data.lastOnGround = playerOnGround;
            }*/
        }

        /*private void OnGrounded()
        {
            if (playerGrounded == false)
            {
                int tickDiff = Server.TickCount - data.lastGroundedTick;
                if (tickDiff == 1)
                {
                    data.lastTickPerfect = true;
                    if (data.lastTickPerfect == true)
                    {
                        data.perfectBhops++;
                        Server.PrintToChatAll($"{player.Controller.PlayerName} Perfect Bhops -> {data.perfectBhops}");

                        if (data.perfectBhops >= 5)
                        {
                            Server.PrintToChatAll($"{player.Controller.PlayerName} might be using bunnyhop hack");
                        }
                    }
                }
                else
                {
                    data.lastTickPerfect = false;
                    data.perfectBhops = 0;
                }
            }
            else
            {
                data.lastGroundedTick = Server.TickCount;
            }

            data.lastGroundedTime = lastLandTime;
        }

        private void OnJump()
        {

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
