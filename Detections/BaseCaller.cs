using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;

namespace TBAntiCheat.Detections
{
    internal static class BaseCaller
    {
        internal static void OnPlayerJoin(PlayerData player)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerJoin(player);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerLeave(PlayerData player)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerLeave(player);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerJump(PlayerData player)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerJump(player);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerHurt(PlayerData victim, PlayerData shooter, HitGroup_t hitgroup)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerHurt(victim, shooter, hitgroup);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerDead(PlayerData victim, PlayerData shooter)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerDead(victim, shooter);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerShoot(PlayerData player)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerShoot(player);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnPlayerTick(PlayerData player)
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnPlayerTick(player);
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnRoundStart()
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnRoundStart();
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnRoundEnd()
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnRoundEnd();
                }
                catch (Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }

        internal static void OnGameTick()
        {
            foreach (BaseDetection detection in Globals.Detections)
            {
                try
                {
                    detection.OnGameTick();
                }
                catch(Exception e)
                {
                    ACCore.Log($"[TBAC] Exception in {detection.Name} -> {e.Message} | {e.StackTrace}");
                }
            }
        }
    }
}
