using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace TBAntiCheat.Handlers
{
    internal static class CommandHandler
    {
        private static BasePlugin? plugin;

        internal static void InitializeCommandHandler(BasePlugin basePlugin)
        {
            plugin = basePlugin;
        }

        internal static bool RegisterCommand(string command, string description, CommandInfo.CommandCallback handler)
        {
            if (plugin == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(command) == true)
            {
                return false;
            }

            if (handler == null)
            {
                return false;
            }

            plugin.AddCommand(command, description, handler);
            return true;
        }
    }
}
