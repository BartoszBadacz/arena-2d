namespace Arena.Gameplay
{
    // Granica Unity ↔ Core: Core liczy na System.Numerics.Vector2, silnik na UnityEngine.Vector2.
    internal static class VectorConversions
    {
        public static System.Numerics.Vector2 ToNumerics(this UnityEngine.Vector2 v) => new System.Numerics.Vector2(v.x, v.y);
        public static UnityEngine.Vector2 ToUnity(this System.Numerics.Vector2 v) => new UnityEngine.Vector2(v.X, v.Y);
    }
}
