using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;
using TBAntiCheat.Utils;

namespace TBAntiCheat.Handlers
{
    internal static class EventHandlers
    {
        internal static void InitializeHandlers(BasePlugin plugin)
        {
            plugin.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

            plugin.RegisterEventHandler<EventPlayerJump>(OnPlayerJump);
            plugin.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

            plugin.RegisterEventHandler<EventWeaponFire>(OnWeaponFire);

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

            if (BanHandler.IsPlayerBanned(player) == true)
            {
                string reason = BanHandler.GetBanReason(player);
                PlayerUtils.KickPlayer(player, reason);

                return HookResult.Continue;
            }

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

        private static HookResult OnPlayerJump(EventPlayerJump jumpEvent, GameEventInfo _)
        {
            if (jumpEvent.Userid == null)
            {
                return HookResult.Continue;
            }

            if (jumpEvent.Userid.IsBot == true)
            {
                return HookResult.Continue;
            }

            if (Globals.Players.ContainsKey(jumpEvent.Userid.Index) == false)
            {
                return HookResult.Continue;
            }

            PlayerData player = Globals.Players[jumpEvent.Userid.Index];
            BaseCaller.OnPlayerJump(player);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerHurt(EventPlayerHurt hurtEvent, GameEventInfo _)
        {
            if (hurtEvent.Userid == null || hurtEvent.Attacker == null)
            {
                return HookResult.Continue;
            }

            if (hurtEvent.Attacker.IsBot == true)
            {
                return HookResult.Continue;
            }

            if (Globals.Players.ContainsKey(hurtEvent.Userid.Index) == false)
            {
                return HookResult.Continue;
            }

            PlayerData victim = Globals.Players[hurtEvent.Userid.Index];
            PlayerData shooter = Globals.Players[hurtEvent.Attacker.Index];

            BaseCaller.OnPlayerHurt(victim, shooter);

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

        private static HookResult OnWeaponFire(EventWeaponFire shootEvent, GameEventInfo _)
        {
            if (shootEvent.Userid == null)
            {
                return HookResult.Continue;
            }

            if (Globals.Players.ContainsKey(shootEvent.Userid.Index) == false)
            {
                return HookResult.Continue;
            }

            PlayerData shooter = Globals.Players[shootEvent.Userid.Index];
            BaseCaller.OnPlayerShoot(shooter);

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
