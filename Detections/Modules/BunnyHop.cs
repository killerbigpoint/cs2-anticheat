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
        internal bool grounded;
        internal int groundedTick;
        internal bool lastTickPerfect;

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
                grounded = true,
                groundedTick = 0,
                lastTickPerfect = false,

                perfectBhops = 0
            });
        }

        internal override void OnPlayerLeave(PlayerData player)
        {
            playerData.Remove(player.Index);
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            BunnyHopData data = playerData[player.Index];
            bool playerGrounded = player.Pawn.OnGroundLastTick;

            if (data.grounded != playerGrounded)
            {
                if (playerGrounded == false)
                {
                    int tickDiff = Server.TickCount - data.groundedTick;
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
                    data.groundedTick = Server.TickCount;
                }

                data.grounded = playerGrounded;
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
