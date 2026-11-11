namespace Arena.Core
{
    // Liczniki jednej rozgrywki (od startu do Game Over). Sam stan — HUD czyta go co klatkę.
    public sealed class RunStats
    {
        public int Kills { get; private set; }
        public float SurvivedTime { get; private set; }
        public int Wave { get; private set; } // 0 = jeszcze przed pierwszą falą

        public void RegisterKill() => Kills++;
        public void AddTime(float deltaTime) => SurvivedTime += deltaTime;
        public void ReachWave(int wave) => Wave = wave;
    }
}
