namespace TBAntiCheat.Detections.Modules
{
    internal class Backtrack : BaseDetection
    {
        internal Backtrack() : base() { }

        internal override string Name => "Backtrack";
        internal override ActionType ActionType => ActionType.Log;
    }
}
