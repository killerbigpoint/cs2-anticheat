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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnPlayerJoin) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnPlayerLeave) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnPlayerDead) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnPlayerTick) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnRoundStart) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnRoundEnd) -> {e.Message}");
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
                    ACCore.Log($"[TBAC] Exception in {detection.Name} (OnGameTick) -> {e.Message}");
                }
            }
        }
    }
}
