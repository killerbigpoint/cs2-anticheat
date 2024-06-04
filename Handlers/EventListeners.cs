using CounterStrikeSharp.API.Core;
using TBAntiCheat.Core;
using TBAntiCheat.Detections;

namespace TBAntiCheat.Handlers
{
    public class EventListeners
    {
        internal static void InitializeListeners(BasePlugin plugin)
        {
            plugin.RegisterListener<Listeners.OnTick>(OnGameTick);
            plugin.RegisterListener<Listeners.OnMapStart>(OnMapStart);
        }

        private static void OnMapStart(string mapName)
        {
            Globals.Initialize();
        }

        private static void OnGameTick()
        {
            BaseCaller.OnGameTick();
        }
    }
}
