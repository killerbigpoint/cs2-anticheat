using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;

namespace TBAntiCheat.Handlers
{
    public static class EventListeners
    {
        internal static void Initialize(BasePlugin plugin)
        {
            plugin.RegisterListener<Listeners.OnTick>(OnGameTick);
            plugin.RegisterListener<Listeners.OnMapStart>(OnMapStart);

            ACCore.Log($"[TBAC] EventListeners Initialized");
        }

        private static void OnMapStart(string mapName)
        {
            Globals.Initialize(false);
        }

        private static void OnGameTick()
        {
            BaseCaller.OnGameTick();

            foreach (PlayerData player in Globals.Players)
            {
                if (player == null)
                {
                    continue;
                }

                BaseCaller.OnPlayerTick(player);
            }
        }
    }
}
