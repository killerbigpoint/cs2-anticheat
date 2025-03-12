using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;

namespace TBAntiCheat.Detections
{
    internal static class BaseCaller
    {
        internal static void OnPlayerJoin(PlayerData player)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerJoin(player);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerLeave(PlayerData player)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerLeave(player);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerJump(PlayerData player)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerJump(player);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerHurt(PlayerData victim, PlayerData shooter, HitGroup_t hitgroup)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerHurt(victim, shooter, hitgroup);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerDead(PlayerData victim, PlayerData shooter)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerDead(victim, shooter);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerShoot(PlayerData player)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerShoot(player);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerTick(PlayerData player)
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnPlayerTick(player);
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnRoundStart()
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnRoundStart();
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnRoundEnd()
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnRoundEnd();
                }
                catch (Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnGameTick()
        {
            foreach (BaseModule module in Globals.Modules)
            {
                try
                {
                    module.OnGameTick();
                }
                catch(Exception e)
                {
                    Globals.Log($"[TBAC] Exception in {module.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }
    }
}
