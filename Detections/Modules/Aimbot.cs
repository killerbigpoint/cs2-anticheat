using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using TBAntiCheat.Core;

namespace TBAntiCheat.Detections.Modules
{
    internal struct AngleSnapshot
    {
        internal float x;
        internal float y;
        internal float z;

        public AngleSnapshot()
        {
            Reset();
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
            detections = 0;
            historyIndex = 0;

            for (int i = 0; i < eyeAngleHistory.Length; i++)
            {
                eyeAngleHistory[i].Reset();
            }
        }
    }

    internal class Aimbot : BaseDetection
    {
        private const float aimbotMinimumDistance = 32f; //Must be 32 units away from the player before we check for aimbot
        private const float aimbotMaxAngle = 30f; //Above 30 degrees in a single tick isn't legit
        private const int aimbotMaxHistory = 64; //1 entire second worth of history
        private const int aimbotMaxDetections = 2; //On 2nd detection we ban

        internal PlayerAimbotData[] eyeAngleHistory;

        internal Aimbot() : base()
        {
            eyeAngleHistory = new PlayerAimbotData[Server.MaxPlayers];
        }

        internal override string Name => "Aimbot";
        internal override ActionType ActionType => actionType;
        private ActionType actionType = ActionType.Log;

        internal override void OnPlayerJoin(PlayerData player)
        {
            eyeAngleHistory[player.Index] = new PlayerAimbotData()
            {
                eyeAngleHistory = new AngleSnapshot[aimbotMaxHistory],
                historyIndex = 0,

                detections = 0
            };
        }

        internal override void OnPlayerLeave(PlayerData player)
        {
            PlayerAimbotData aimbotData = eyeAngleHistory[player.Index];
            aimbotData.Reset();
        }

        internal override void OnPlayerDead(PlayerData victim, PlayerData shooter)
        {
            if (victim.Pawn.AbsOrigin == null || shooter.Pawn.AbsOrigin == null)
            {
                return;
            }

            AngleSnapshot victimPos = new AngleSnapshot(victim.Pawn.AbsOrigin);
            AngleSnapshot shooterPos = new AngleSnapshot(shooter.Pawn.AbsOrigin);

            float distance = AngleSnapshot.Distance(victimPos, shooterPos);
            if (distance < aimbotMinimumDistance)
            {
                return;
            }

            PlayerAimbotData aimbotData = eyeAngleHistory[shooter.Index];

            int historyIndex = aimbotData.historyIndex;
            AngleSnapshot lastAngle = aimbotData.eyeAngleHistory[historyIndex];

            for (int i = 1; i < aimbotMaxHistory; i++)
            {
                if (historyIndex == aimbotMaxHistory)
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

                if (angleDiff > aimbotMaxAngle)
                {
                    OnAimbotDetected(shooter, aimbotData, angleDiff);
                    break;
                }

                lastAngle = currentAngle;
                historyIndex++;
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

        internal override void OnGameTick()
        {
            foreach (KeyValuePair<uint, PlayerData> player in Globals.Players)
            {
                PlayerData playerData = player.Value;
                PlayerAimbotData aimbotData = eyeAngleHistory[playerData.Index];

                AngleSnapshot snapshot = new AngleSnapshot(playerData.Pawn.EyeAngles);
                aimbotData.eyeAngleHistory[aimbotData.historyIndex] = snapshot;

                aimbotData.historyIndex++;
                if (aimbotData.historyIndex == aimbotMaxHistory)
                {
                    aimbotData.historyIndex = 0;
                }
            }
        }

        private void OnAimbotDetected(PlayerData player, PlayerAimbotData data, float angleDiff)
        {
            data.detections++;
            ACCore.Log($"[TBAC] {player.Controller.PlayerName}: Suspicious aimbot -> {angleDiff} degrees ({data.detections}/{aimbotMaxDetections} detections)");

            if (data.detections < aimbotMaxDetections)
            {
                return;
            }

            OnPlayerDetected(player);
        }
    }
}
