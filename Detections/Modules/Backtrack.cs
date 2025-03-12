namespace TBAntiCheat.Detections.Modules
{

    /*
     * Module: Backtrack
     * Purpose: Detect players which abuse the tick system by modifying it to a previous tick, ensuring a kill which is not even on the target
     * NOTE: This has drastically changed in Source 2 as far as I know but some research needs to be done regarding this before an implementation is made
     */
    internal class Backtrack : BaseModule
    {
        internal override string Name => "Backtrack";
        internal override ActionType ActionType => ActionType.Log;
        internal override bool AlertDiscord => false;

        internal Backtrack() : base() { }
    }
}
