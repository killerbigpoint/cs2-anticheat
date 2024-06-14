using TBAntiCheat.Core;

namespace TBAntiCheat.Detections
{
    public enum ActionType : byte
    {
        None = 0,
        Log = 1,
        Kick = 2,
        Ban = 3
    }

    internal abstract class BaseDetection
    {
        internal BaseDetection() { }

        internal abstract string Name { get; }
        internal abstract ActionType ActionType { get; }

        protected void OnPlayerDetected(PlayerData player, string reason)
        {
            DetectionMetadata metadata = new DetectionMetadata()
            {
                detection = this,
                player = player,
                time = DateTime.Now,
                reason = reason
            };

            DetectionHandler.OnPlayerDetected(metadata);
        }

        internal virtual void OnPlayerJoin(PlayerData player) { }
        internal virtual void OnPlayerLeave(PlayerData player) { }

        internal virtual void OnPlayerJump(PlayerData player) { }
        internal virtual void OnPlayerHurt(PlayerData victim, PlayerData shooter) { }
        internal virtual void OnPlayerDead(PlayerData victim, PlayerData shooter) { }
        internal virtual void OnPlayerShoot(PlayerData player) { }
        internal virtual void OnPlayerTick(PlayerData player) { }

        internal virtual void OnRoundStart() { }
        internal virtual void OnRoundEnd() { }

        internal virtual void OnGameTick() { }
    }
}
