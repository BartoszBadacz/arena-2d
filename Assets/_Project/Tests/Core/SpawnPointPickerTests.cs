using System;
using System.Numerics;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class SpawnPointPickerTests
    {
        private static readonly Vector2 HalfSize = new Vector2(13f, 7f);
        private const float MinDistance = 6f;

        [Test]
        public void PointsLieOnArenaEdge()
        {
            var picker = new SpawnPointPicker(HalfSize, MinDistance, new Random(1));

            for (int i = 0; i < 200; i++)
            {
                Vector2 p = picker.Pick(Vector2.Zero);
                bool onEdge = Math.Abs(Math.Abs(p.X) - HalfSize.X) < 0.0001f || Math.Abs(Math.Abs(p.Y) - HalfSize.Y) < 0.0001f;
                Assert.IsTrue(onEdge, $"punkt {p} nie leży na krawędzi");
            }
        }

        [Test]
        public void PointsKeepMinDistanceFromPlayer()
        {
            var picker = new SpawnPointPicker(HalfSize, MinDistance, new Random(2));
            var player = new Vector2(12f, 6f); // w rogu — duża część krawędzi jest za blisko

            for (int i = 0; i < 200; i++)
                Assert.GreaterOrEqual(Vector2.Distance(picker.Pick(player), player), MinDistance);
        }

        [Test]
        public void ImpossibleDistance_FallsBackToOppositeCorner()
        {
            var picker = new SpawnPointPicker(HalfSize, minDistanceFromPlayer: 100f, new Random(3));

            Vector2 p = picker.Pick(new Vector2(5f, -2f));

            Assert.AreEqual(new Vector2(-13f, 7f), p);
        }
    }
}
