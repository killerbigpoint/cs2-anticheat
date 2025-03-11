using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;

namespace TBAntiCheat.Handlers
{
    internal static class EventHandlers
    {
        internal static void Initialize(BasePlugin plugin, bool hotReload)
        {
            plugin.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            plugin.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect, HookMode.Pre);
            plugin.RegisterEventHandler<EventPlayerActivate>(OnPlayerActivate);

            plugin.RegisterEventHandler<EventPlayerJump>(OnPlayerJump);
            plugin.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
            plugin.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);

            plugin.RegisterEventHandler<EventWeaponFire>(OnWeaponFire);

            plugin.RegisterEventHandler<EventRoundStart>(OnRoundStart);
            plugin.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);

            Globals.Log($"[TBAC] EventHandlers Initialized");

            if (hotReload == true)
            {
                for (int i = 0; i < Server.MaxPlayers; i++)
                {
                    CCSPlayerController? controller = Utilities.GetPlayerFromSlot(i);
                    if (controller == null || controller.IsValid == false)
                    {
                        continue;
                    }

                    if (controller.Connected != PlayerConnectedState.PlayerConnected)
                    {
                        continue;
                    }

                    OnPlayerJoined(controller);
                }
            }
        }

        private static HookResult OnPlayerActivate(EventPlayerActivate activateEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = activateEvent.Userid;
            if (controller == null || controller.IsValid == false)
            {
                return HookResult.Continue;
            }

            if (controller.IsBot == false)
            {
                return HookResult.Continue;
            }

            Globals.Log($"[TBAC] Bot activated -> {controller.Slot} | {controller.PlayerName}");
            OnPlayerJoined(controller);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerConnectFull(EventPlayerConnectFull connectEvent, GameEventInfo _)
        {
            OnPlayerJoined(connectEvent.Userid);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerDisconnect(EventPlayerDisconnect disconnectEvent, GameEventInfo _)
        {
            OnPlayerLeft(disconnectEvent.Userid);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerJump(EventPlayerJump jumpEvent, GameEventInfo _)
        {
            CCSPlayerController? controller = jumpEvent.Userid;
            if (controller == null || controller.IsValid == false)
            {
                return HookResult.Continue;
            }

            PlayerData player = Globals.Players[controller.Slot];
            BaseCaller.OnPlayerJump(player);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerHurt(EventPlayerHurt hurtEvent, GameEventInfo _)
        {
            CCSPlayerController? victimController = hurtEvent.Userid;
            CCSPlayerController? shooterController = hurtEvent.Attacker;

            if (victimController == null || victimController.IsValid == false ||
                shooterController == null || shooterController.IsValid == false)
            {
                return HookResult.Continue;
            }

            PlayerData victim = Globals.Players[victimController.Slot];
            PlayerData shooter = Globals.Players[shooterController.Slot];

            BaseCaller.OnPlayerHurt(victim, shooter, (HitGroup_t)hurtEvent.Hitgroup);

            return HookResult.Continue;
        }

        private static HookResult OnPlayerDeath(EventPlayerDeath deathEvent, GameEventInfo _)
        {
            CCSPlayerController? victimController = deathEvent.Userid;
            CCSPlayerController? shooterController = deathEvent.Attacker;

            if (victimController == null || victimController.IsValid == false ||
                shooterController == null || shooterController.IsValid == false)
            {
                return HookResult.Continue;
            }

            PlayerData victim = Globals.Players[victimController.Slot];
            PlayerData shooter = Globals.Players[shooterController.Slot];

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
            if (controller == null || controller.IsValid == false)
            {
                return HookResult.Continue;
            }

            PlayerData shooter = Globals.Players[controller.Slot];
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

        // ----- Helper Functions ----- \\

        private static void OnPlayerJoined(CCSPlayerController? controller)
        {
            if (controller == null || controller.IsValid == false)
            {
                Globals.Log($"[TBAC] WARNING: Controller is invalid when player joined");
                return;
            }

            CCSPlayerPawn? pawn = controller.PlayerPawn.Value;
            if (pawn == null)
            {
                Globals.Log($"[TBAC] WARNING: Pawn is invalid when player joined");
                return;
            }

            int playerIndex = controller.Slot;
            PlayerData player = new PlayerData()
            {
                Controller = controller,
                Pawn = pawn,

                Index = playerIndex,
                IsBot = controller.IsBot
            };

            Globals.Players[playerIndex] = player;
            BaseCaller.OnPlayerJoin(player);

            Globals.Log($"[TBAC] Player joined -> {playerIndex} | {controller.PlayerName}");
        }

        private static void OnPlayerLeft(CCSPlayerController? controller)
        {
            if (controller == null || controller.IsValid == false)
            {
                Globals.Log($"[TBAC] WARNING: Controller is invalid when player left");
                return;
            }

            CCSPlayerPawn? pawn = controller.PlayerPawn.Value;
            if (pawn == null)
            {
                Globals.Log($"[TBAC] WARNING: Pawn is invalid when player left");
                return;
            }

            int playerIndex = controller.Slot;
            PlayerData player = Globals.Players[playerIndex];

            BaseCaller.OnPlayerLeave(player);
            Globals.Players[playerIndex] = null!;

            Globals.Log($"[TBAC] Player left -> {playerIndex} | {controller.PlayerName}");
        }
    }
}
