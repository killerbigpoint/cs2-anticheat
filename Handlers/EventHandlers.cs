using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;

namespace TBAntiCheat.Handlers
{
    internal static class EventHandlers
    {
        internal static void InitializeHandlers(BasePlugin plugin)
        {
            plugin.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);
            plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

            plugin.RegisterEventHandler<EventRoundStart>(OnRoundStart);
            plugin.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        }

        private static HookResult OnPlayerConnectFull(EventPlayerConnectFull connectEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = connectEvent.Userid;
            if (controller == null || controller.IsValid == false)
            {
                return HookResult.Continue;
            }

            CCSPlayerPawn? pawn = controller.PlayerPawn.Value;
            if (pawn == null)
            {
                return HookResult.Continue;
            }

            PlayerData player = new PlayerData()
            {
                Controller = controller,
                Pawn = pawn,

                Index = (int)controller.Index
            };

            Globals.Players.Add(controller.Index, player);
            BaseCaller.OnPlayerJoin(player);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerDisconnect(EventPlayerDisconnect connectEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = connectEvent.Userid;
            if (controller == null || controller.IsValid == false)
            {
                return HookResult.Continue;
            }

            if (controller.IsBot == true)
            {
                return HookResult.Continue;
            }

            PlayerData player = Globals.Players[controller.Index];

            BaseCaller.OnPlayerLeave(player);
            Globals.Players.Remove(controller.Index);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerDeath(EventPlayerDeath deathEvent, GameEventInfo _)
        {
            if (deathEvent.Userid == null || deathEvent.Attacker == null)
            {
                return HookResult.Continue;
            }

            if (deathEvent.Attacker.IsBot == true)
            {
                return HookResult.Continue;
            }

            if (Globals.Players.ContainsKey(deathEvent.Userid.Index) == false)
            {
                return HookResult.Continue;
            }

            PlayerData victim = Globals.Players[deathEvent.Userid.Index];
            PlayerData shooter = Globals.Players[deathEvent.Attacker.Index];

            BaseCaller.OnPlayerDead(victim, shooter);

            return HookResult.Continue;
        }

        private static HookResult OnRoundStart(EventRoundStart roundStartEvent, GameEventInfo _)
        {
            BaseCaller.OnRoundStart();

            return HookResult.Continue;
        }

        private static HookResult OnRoundEnd(EventRoundEnd roundEndEvent, GameEventInfo _)
        {
            BaseCaller.OnRoundEnd();

            return HookResult.Continue;
        }
    }
}
