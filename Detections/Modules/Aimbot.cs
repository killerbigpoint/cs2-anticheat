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
        private readonly PlayerAimbotData[] eyeAngleHistory;

        internal Aimbot() : base()
        {
            config = new BaseConfig<AimbotSaveData>("Aimbot");
            eyeAngleHistory = new PlayerAimbotData[Server.MaxPlayers];

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

            eyeAngleHistory[player.Index] = new PlayerAimbotData()
            {
                eyeAngleHistory = new QAngle[aimbotMaxHistory],
                historyIndex = 0,

                detections = 0
            };
        }

        internal override void OnPlayerLeave(PlayerData player)
        {
            if (player.Controller.IsBot == true)
            {
                return;
            }
        }

        /*internal override void OnPlayerHurt(PlayerData victim, PlayerData shooter, HitGroup_t hitgroup)
        {
            //TODO: This ain't ready yet at all. Needs more work

            //You might be wondering, why the fuck are we converting it to Quaternion and then back to a Vector/AngleSnapshot?
            //Well let me tell you, because I simply suck at math and CounterStrikeSharp don't provide any way to get a bone position
            //So here we find the approximate offset from the abs origin of each bone via the use of some complicated math that I totally
            //Didn't steal from Unity and some StackOverflow posts. But hey, it gives us exactly what we want for a "proper" angle
            Quaternion originAbsPos = new Quaternion(shooter.Pawn.AbsOrigin);
            AngleSnapshot originHead = originAbsPos * originOffsetHead;

            AngleSnapshot targetAbs = new AngleSnapshot(victim.Pawn.AbsOrigin);
            Quaternion targetAbsQuat = new Quaternion(targetAbs);

            AngleSnapshot targetPos = hitgroup switch
            {
                HitGroup_t.HITGROUP_HEAD => targetAbsQuat * originOffsetHead,
                HitGroup_t.HITGROUP_CHEST => targetAbsQuat * originOffsetStomach,
                HitGroup_t.HITGROUP_STOMACH => targetAbsQuat * originOffsetChest,
                HitGroup_t.HITGROUP_LEFTARM => targetAbsQuat * originOffsetLeftArm,
                HitGroup_t.HITGROUP_RIGHTARM => targetAbsQuat * originOffsetRightArm,
                HitGroup_t.HITGROUP_LEFTLEG => targetAbsQuat * originOffsetLeftLeg,
                HitGroup_t.HITGROUP_RIGHTLEG => targetAbsQuat * originOffsetRightLeg,
                _ => targetAbs,
            };

            CWeaponMP9? mp9 = Utilities.CreateEntityByName<CWeaponMP9>("weapon_mp9");
            if (mp9 != null)
            {
                Vector enginePos = new Vector(targetPos.x, targetPos.y, targetPos.z);
                mp9.Teleport();
            }
            else
            {
                Server.PrintToChatAll($"MP9 is null");
            }

            AngleSnapshot shotEyeAngles = AngleSnapshot.Direction(originHead, targetPos);

            //NOTE: To increase the validity of this in the future
            //We probably need to account for spread too, how though? I have no idea as of now but we'll figure it out like always
            //AngleSnapshot calculatedSpread = new AngleSnapshot();
            //shotEyeAngles += calculatedSpread;

            PlayerAimbotData aimbotData = eyeAngleHistory[shooter.Index];
            aimbotData.eyeAngleHistory[aimbotData.historyIndex] = shotEyeAngles;

            Server.PrintToChatAll($"Hurt: {shooter.Controller.PlayerName} -> {shotEyeAngles} ({hitgroup})");
        }*/

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

            PlayerAimbotData aimbotData = eyeAngleHistory[player.Index];

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
                if (data == null)
                {
                    continue;
                }

                PlayerAimbotData aimbotData = eyeAngleHistory[data.Index];
                if (aimbotData == null)
                {
                    continue;
                }

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
