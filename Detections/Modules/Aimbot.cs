using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;
using TBAntiCheat.Handlers;

namespace TBAntiCheat.Detections.Modules
{
    public class AimbotSaveData
    {
        public bool DetectionEnabled { get; set; } = true;
        public ActionType DetectionAction { get; set; } = ActionType.Kick;

        public float MaxAimbotAngle { get; set; } = 30f;
        public int MaxDetectionsBeforeAction { get; set; } = 2;
    }

    internal class PlayerAimbotData
    {
        internal required QAngle[] eyeAngleHistory;
        internal required int historyIndex;

        internal required int detections;

        internal void Reset()
        {
            historyIndex = 0;
        }
    }

    /*
     * Module: Aimbot
     * Purpose: Detect players which flick with their eye angles at high velocity. Nobody can reliably flick 20+ degrees in a single tick and still hit players
     * NOTE: For some reason EyeAngles is not properly synched from client to server I think? Either way this module will always be unreliable against rage hackers
     */
    internal class Aimbot : BaseDetection
    {
        private const int aimbotMaxHistory = 64; //1 entire second worth of history (considering the tickrate is 64)

        private readonly BaseConfig<AimbotSaveData> config;
        private readonly PlayerAimbotData[] playerData;

        internal Aimbot() : base()
        {
            config = new BaseConfig<AimbotSaveData>("Aimbot");
            playerData = new PlayerAimbotData[64];

            CommandHandler.RegisterCommand("tbac_aimbot_enable", "Activates/Deactivates the aimbot detection", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_angle", "Max angle in a single tick before detection", OnAngleCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_detections", "Maximum detections before an action should be taken", OnDetectionsCommand);

            ACCore.Log($"[TBAC] Aimbot Initialized");
        }

        internal override string Name => "Aimbot";
        internal override ActionType ActionType => config.Config.DetectionAction;

        internal override void OnPlayerJoin(PlayerData player)
        {
            if (player.Controller.IsBot == true)
            {
                return;
            }

            playerData[player.Index] = new PlayerAimbotData()
            {
                eyeAngleHistory = new QAngle[aimbotMaxHistory],
                historyIndex = 0,

                detections = 0
            };

            ACCore.Log($"[TBAC] Initialized player with index {player.Index}");
        }

        internal override void OnPlayerLeave(PlayerData player)
        {
            if (player.Controller.IsBot == true)
            {
                return;
            }

            ACCore.Log($"[TBAC] Disposed player with index {player.Index}");
        }

        internal override void OnPlayerDead(PlayerData victim, PlayerData shooter)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            if (shooter.Controller.IsBot == true)
            {
                return;
            }

            if (victim.Pawn.AbsOrigin == null || shooter.Pawn.AbsOrigin == null)
            {
                ACCore.Log("[TBAC] Something is wrong here. AbsOrigin on either of these is null");
                return;
            }

            PlayerAimbotData aimbotData = playerData[shooter.Index];

            int historyIndex = aimbotData.historyIndex;
            QAngle lastAngle = aimbotData.eyeAngleHistory[historyIndex];

            float maxAngle = config.Config.MaxAimbotAngle;

            historyIndex++;
            for (int i = historyIndex; i < aimbotMaxHistory; i++)
            {
                if (historyIndex >= aimbotMaxHistory)
                {
                    historyIndex = 0;
                }

                QAngle currentAngle = aimbotData.eyeAngleHistory[historyIndex];
                float angleDiff = Distance(lastAngle, currentAngle);

                //Normalize the angle so we can use it for our aimbot detection logic
                if (angleDiff > 180f)
                {
                    angleDiff = MathF.Abs(angleDiff - 360);
                }

                //Server.PrintToChatAll($"{i}: {shooter.Controller.PlayerName} -> {angleDiff}");

                if (angleDiff > maxAngle)
                {
                    OnAimbotDetected(shooter, aimbotData, angleDiff);
                    break;
                }

                lastAngle = currentAngle;
                historyIndex++;
            }
        }

        internal override void OnPlayerTick(PlayerData player)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            if (player.Controller.IsBot == true)
            {
                return;
            }

            //Safeguard against error spam for now
            if (player.NumErrors >= 5)
            {
                return;
            }

            PlayerAimbotData aimbotData;
            try
            {
                aimbotData = playerData[player.Index];
            }
            catch
            {
                ACCore.Log($"[TBAC] WARNING: {player.Controller.PlayerName} ({player.Index}) is OOB. Report this to the dev!");
                player.NumErrors++;

                return;
            }

            QAngle eyeAngles = player.Pawn.EyeAngles;
            QAngle snapshot = new QAngle(eyeAngles.X, eyeAngles.Y, eyeAngles.Z);
            aimbotData.eyeAngleHistory[aimbotData.historyIndex] = snapshot;

            aimbotData.historyIndex++;
            if (aimbotData.historyIndex == aimbotMaxHistory)
            {
                aimbotData.historyIndex = 0;
            }
        }

        internal override void OnRoundStart()
        {
            foreach (KeyValuePair<uint, PlayerData> player in Globals.Players)
            {
                PlayerData data = player.Value;
                if (data.Controller.IsBot == true)
                {
                    continue;
                }

                PlayerAimbotData aimbotData = playerData[data.Index];
                aimbotData.Reset();
            }
        }

        private void OnAimbotDetected(PlayerData player, PlayerAimbotData data, float angleDiff)
        {
            int maxDetections = config.Config.MaxDetectionsBeforeAction;

            data.detections++;
            ACCore.Log($"[TBAC] {player.Controller.PlayerName}: Suspicious aimbot -> {angleDiff} degrees ({data.detections}/{maxDetections} detections)");

            if (data.detections < maxDetections)
            {
                return;
            }

            string reason = $"Aimbot -> {angleDiff}";
            OnPlayerDetected(player, reason);
        }

        // ----- Commands ----- \\

        [RequiresPermissions("@css/admin")]
        private void OnEnableCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (bool.TryParse(arg, out bool state) == false)
            {
                return;
            }

            config.Config.DetectionEnabled = state;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnActionCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (int.TryParse(arg, out int action) == false)
            {
                return;
            }

            ActionType actionType = (ActionType)action;
            if (config.Config.DetectionAction.HasFlag(actionType) == false)
            {
                return;
            }

            config.Config.DetectionAction = actionType;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnAngleCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (float.TryParse(arg, out float angle) == false)
            {
                return;
            }

            config.Config.MaxAimbotAngle = angle;
            config.Save();
        }

        [RequiresPermissions("@css/admin")]
        private void OnDetectionsCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (command.ArgCount != 2)
            {
                return;
            }

            string arg = command.ArgByIndex(1);
            if (int.TryParse(arg, out int detections) == false)
            {
                return;
            }

            config.Config.MaxDetectionsBeforeAction = detections;
            config.Save();
        }

        // ----- Helper Functions ----- \\

        public static float Distance(QAngle a, QAngle b)
        {
            float newX = a.X - b.X;
            float newY = a.Y - b.Y;
            float newZ = a.Z - b.Z;

            float distance = MathF.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return distance;
        }
    }
}
