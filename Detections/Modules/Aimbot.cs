﻿using CounterStrikeSharp.API;
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

    internal struct AngleSnapshot
    {
        internal float x;
        internal float y;
        internal float z;

        public AngleSnapshot()
        {
            Reset();
        }

        public AngleSnapshot(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public AngleSnapshot(Vector? vector)
        {
            if (vector == null)
            {
                Reset();
                return;
            }

            x = vector.X;
            y = vector.Y;
            z = vector.Z;
        }

        public AngleSnapshot(QAngle? angle)
        {
            if (angle == null)
            {
                Reset();
                return;
            }

            x = angle.X;
            y = angle.Y;
            z = angle.Z;
        }

        public static float Distance(AngleSnapshot a, AngleSnapshot b)
        {
            float newX = a.x - b.x;
            float newY = a.y - b.y;
            float newZ = a.z - b.z;

            float distance = MathF.Sqrt(newX * newX + newY * newY + newZ * newZ);
            return distance;
        }

        public static AngleSnapshot Direction(AngleSnapshot origin, AngleSnapshot target)
        {
            AngleSnapshot direction = target - origin;
            return direction.Normalize();
        }

        public static AngleSnapshot operator -(AngleSnapshot a, AngleSnapshot b)
        {
            return new AngleSnapshot(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public readonly float Length()
        {
            return MathF.Sqrt(x * x + y * y + z * z);
        }

        public readonly AngleSnapshot Normalize()
        {
            float length = Length();
            if (length == 0)
            {
                return new AngleSnapshot(0, 0, 0); //Avoid division by zero
            }

            return new AngleSnapshot(x / length, y / length, z / length);
        }

        internal void Reset()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public override readonly string ToString()
        {
            return $"{x:n2} {y:n2} {z:n2}";
        }
    }

    internal class PlayerAimbotData
    {
        internal required AngleSnapshot[] eyeAngleHistory;
        internal required int historyIndex;

        internal required int detections;

        internal void Reset()
        {
            historyIndex = 0;

            for (int i = 0; i < eyeAngleHistory.Length; i++)
            {
                eyeAngleHistory[i].Reset();
            }
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
        private readonly PlayerAimbotData[] eyeAngleHistory;

        internal Aimbot() : base()
        {
            config = new BaseConfig<AimbotSaveData>("Aimbot");
            eyeAngleHistory = new PlayerAimbotData[Server.MaxPlayers];

            CommandHandler.RegisterCommand("tbac_aimbot_enable", "Activates/Deactivates the aimbot detection", OnEnableCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_action", "Which action to take on the player. 0 = none | 1 = log | 2 = kick | 3 = ban", OnActionCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_angle", "Max angle in a single tick before detection", OnAngleCommand);
            CommandHandler.RegisterCommand("tbac_aimbot_detections", "Maximum detections before an action should be taken", OnDetectionsCommand);
        }

        internal override string Name => "Aimbot";
        internal override ActionType ActionType => config.Config.DetectionAction;

        internal override void OnPlayerJoin(PlayerData player)
        {
            eyeAngleHistory[player.Index] = new PlayerAimbotData()
            {
                eyeAngleHistory = new AngleSnapshot[aimbotMaxHistory],
                historyIndex = 0,

                detections = 0
            };
        }

        internal override void OnPlayerHurt(PlayerData victim, PlayerData shooter)
        {
            //NOTE: To increase the validity of this in the future
            //We probably need to account for spread too, how though? I have no idea as of now but we'll figure it out like always

            AngleSnapshot origin = new (shooter.Pawn.AbsOrigin);
            AngleSnapshot target = new (victim.Pawn.AbsOrigin);

            AngleSnapshot shotEyeAngles = AngleSnapshot.Direction(origin, target);

            PlayerAimbotData aimbotData = eyeAngleHistory[shooter.Index];
            aimbotData.eyeAngleHistory[aimbotData.historyIndex] = shotEyeAngles;

            Server.PrintToChatAll($"Hurt: {shooter.Controller.PlayerName} -> {shotEyeAngles}");
        }

        internal override void OnPlayerDead(PlayerData victim, PlayerData shooter)
        {
            if (config.Config.DetectionEnabled == false)
            {
                return;
            }

            if (victim.Pawn.AbsOrigin == null || shooter.Pawn.AbsOrigin == null)
            {
                return;
            }

            PlayerAimbotData aimbotData = eyeAngleHistory[shooter.Index];

            int historyIndex = aimbotData.historyIndex;
            AngleSnapshot lastAngle = aimbotData.eyeAngleHistory[historyIndex];

            float maxAngle = config.Config.MaxAimbotAngle;

            historyIndex++;
            for (int i = historyIndex; i < aimbotMaxHistory; i++)
            {
                if (historyIndex >= aimbotMaxHistory)
                {
                    historyIndex = 0;
                }

                AngleSnapshot currentAngle = aimbotData.eyeAngleHistory[historyIndex];
                float angleDiff = AngleSnapshot.Distance(lastAngle, currentAngle);

                //Normalize the angle so we can use it for our aimbot detection logic
                if (angleDiff > 180f)
                {
                    angleDiff = MathF.Abs(angleDiff - 360);
                }

                Server.PrintToChatAll($"{i}: {shooter.Controller.PlayerName} -> {angleDiff}");

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

            PlayerAimbotData aimbotData = eyeAngleHistory[player.Index];

            AngleSnapshot snapshot = new AngleSnapshot(player.Pawn.EyeAngles);
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
                PlayerAimbotData aimbotData = eyeAngleHistory[data.Index];

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
    }
}
