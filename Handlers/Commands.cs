using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using TBAntiCheat.Core;

namespace TBAntiCheat.Handlers
{
    internal class Commands
    {
        internal static void InitializeCommands(BasePlugin plugin)
        {
            plugin.AddCommand("tbac_enable", "Activates/Deactivates the anticheat", OnEnableCommand);
        }

        [RequiresPermissions("@css/admin")]
        private static void OnEnableCommand(CCSPlayerController? player, CommandInfo command)
        {
            ACCore.Log($"[TBAC] {command.ArgCount}");

            for (int i = 0; i < command.ArgCount; i++)
            {
                ACCore.Log($"[TBAC] {command.ArgByIndex(i)}");
            }
        }
    }
}
