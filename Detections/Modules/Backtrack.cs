namespace TBAntiCheat.Detections.Modules
{
    internal class Backtrack : BaseDetection
    {
        internal Backtrack() : base() { }

        internal override string Name => "Backtrack";
        internal override ActionType ActionType => actionType;
        private ActionType actionType = ActionType.Log;

        /*internal override void OnGameTick()
        {
            foreach (KeyValuePair<uint, PlayerData> player in Globals.Players)
            {
                //TODO: Check simulation time and ticks here
                //NOTE: Since backtracking isn't purely based on tickbase anymore we need to figure out a smarter way to handle this
            }
        }*/
    }
}
