namespace COMMO.GameServer.Items {
	public struct SharedItemAbilities {
        public uint HealthGain;
        public uint HealthTicks;
        public uint ManaGain;
        public uint ManaTicks;

        public bool ManaShield;
        public bool PreventDrop;
        public bool PreventLoss;

        public void Reset() {
            HealthGain = HealthTicks = ManaGain = ManaTicks = 0;
            ManaShield = PreventDrop = PreventLoss = false;
        }
    }
}