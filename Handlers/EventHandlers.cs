using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;
using TBAntiCheat.Utils;

namespace TBAntiCheat.Handlers
{
    internal static class EventHandlers
    {
        internal static void Initialize(BasePlugin plugin, bool hotReload)
        {
            plugin.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

            plugin.RegisterEventHandler<EventPlayerJump>(OnPlayerJump);
            plugin.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

            plugin.RegisterEventHandler<EventWeaponFire>(OnWeaponFire);

            plugin.RegisterEventHandler<EventRoundStart>(OnRoundStart);
            plugin.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);

            if (hotReload == true)
            {
                foreach (CCSPlayerController controller in Utilities.GetPlayers())
                {
                    JoinPlayerImpl(controller);
                }
            }

            ACCore.Log($"[TBAC] EventHandlers Initialized");
        }

        private static HookResult OnPlayerConnectFull(EventPlayerConnectFull connectEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = connectEvent.Userid;
            if (controller == null || controller.IsValid == false || controller.IsBot == true)
            {
                return HookResult.Continue;
            }

            JoinPlayerImpl(controller);

            return HookResult.Continue;
        }

        private static void JoinPlayerImpl(CCSPlayerController controller)
        {
            CCSPlayerPawn? pawn = controller.PlayerPawn.Value;
            if (pawn == null)
            {
                return;
            }

            // We need to subtract 1 here because player indexes always start at 1.
            // This is because the world also has an entity index which is 0.
            // So to properly use arrays we need to pretend the indexes are 1 less
            int properIndex = (int)controller.Index - 1;
            PlayerData player = new PlayerData()
            {
                Controller = controller,
                Pawn = pawn,

                Index = properIndex
            };

            if (BanHandler.IsPlayerBanned(player) == true)
            {
                string reason = BanHandler.GetBanReason(player);
                PlayerUtils.KickPlayer(player, reason);

                return;
            }

            Globals.Players[properIndex] = player;
            Globals.PlayerReverseLookup[(int)pawn.Index] = properIndex;

            BaseCaller.OnPlayerJoin(player);

            ACCore.Log($"[TBAC] Initialized player with index {player.Index} (Pawn: {pawn.Index})");
        }

        private static HookResult OnPlayerDisconnect(EventPlayerDisconnect connectEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = connectEvent.Userid;
            if (controller == null || controller.IsValid == false || controller.IsBot == true)
            {
                return HookResult.Continue;
            }

            CCSPlayerPawn? pawn = controller.PlayerPawn.Value;
            if (pawn == null)
            {
                return HookResult.Continue;
            }

            int properIndex = (int)controller.Index - 1;
            PlayerData player = Globals.Players[properIndex];

            BaseCaller.OnPlayerLeave(player);

            Globals.Players[properIndex] = null!;
            Globals.PlayerReverseLookup[(int)pawn.Index] = -1;

            ACCore.Log($"[TBAC] Disposed player with index {player.Index} (Pawn: {pawn.Index})");

            return HookResult.Continue;
        }

        private static HookResult OnPlayerJump(EventPlayerJump jumpEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = jumpEvent.Userid;
            if (controller == null || controller.IsValid == false || controller.IsBot == true)
            {
                return HookResult.Continue;
            }

            int properIndex = (int)controller.Index - 1;
            PlayerData player = Globals.Players[properIndex];

            BaseCaller.OnPlayerJump(player);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerHurt(EventPlayerHurt hurtEvent, GameEventInfo _)
        {
            CCSPlayerController? victimController = hurtEvent.Userid;
            CCSPlayerController? shooterController = hurtEvent.Attacker;

            if (victimController == null || victimController.IsValid == false || victimController.IsBot == true ||
                shooterController == null || shooterController.IsValid == false || shooterController.IsBot == true)
            {
                return HookResult.Continue;
            }

            int properIndexVictim = (int)victimController.Index - 1;
            int properIndexShooter = (int)shooterController.Index - 1;

            PlayerData victim = Globals.Players[properIndexVictim];
            PlayerData shooter = Globals.Players[properIndexShooter];

            BaseCaller.OnPlayerHurt(victim, shooter, (HitGroup_t)hurtEvent.Hitgroup);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerDeath(EventPlayerDeath deathEvent, GameEventInfo _)
        {
            CCSPlayerController? victimController = deathEvent.Userid;
            CCSPlayerController? shooterController = deathEvent.Attacker;

            if (victimController == null || victimController.IsValid == false || victimController.IsBot == true ||
                shooterController == null || shooterController.IsValid == false || shooterController.IsBot == true)
            {
                return HookResult.Continue;
            }

            int properIndexVictim = (int)victimController.Index - 1;
            int properIndexShooter = (int)shooterController.Index - 1;

            PlayerData victim = Globals.Players[properIndexVictim];
            PlayerData shooter = Globals.Players[properIndexShooter];

            if (victim == null || shooter == null)
            {
                return HookResult.Continue;
            }

            BaseCaller.OnPlayerDead(victim, shooter);

            return HookResult.Continue;
        }

        private static HookResult OnWeaponFire(EventWeaponFire shootEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = shootEvent.Userid;
            if (controller == null || controller.IsValid == false || controller.IsBot == true)
            {
                return HookResult.Continue;
            }

            int properIndex = (int)controller.Index - 1;
            PlayerData shooter = Globals.Players[properIndex];

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
