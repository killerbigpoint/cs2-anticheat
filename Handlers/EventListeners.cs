using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;

namespace TBAntiCheat.Handlers
{
    public static class EventListeners
    {
        internal static void InitializeListeners(BasePlugin plugin)
        {
            plugin.RegisterListener<Listeners.OnTick>(OnGameTick);
            plugin.RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }

        private static void OnMapStart(string mapName)
        {
            Globals.Initialize(false);
        }

        private static void OnGameTick()
        {
            BaseCaller.OnGameTick();

            foreach (KeyValuePair<uint, PlayerData> player in Globals.Players)
            {
                BaseCaller.OnPlayerTick(player.Value);
            }
        }
    }
}
