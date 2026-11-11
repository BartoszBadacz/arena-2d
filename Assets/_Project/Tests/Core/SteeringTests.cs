using System;
using System.Numerics;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class SteeringTests
    {
        private const float Tolerance = 0.0001f;
        private readonly Random random = new Random(42);

        [Test]
        public void Separation_WithoutNeighbours_IsZero()
        {
            Vector2 push = Steering.Separation(Vector2.Zero, ReadOnlySpan<Vector2>.Empty, radius: 2f, random);

            Assert.AreEqual(Vector2.Zero, push);
        }

        [Test]
        public void Separation_PushesAwayFromNeighbour()
        {
            Vector2[] neighbours = { new Vector2(1f, 0f) };

            Vector2 push = Steering.Separation(Vector2.Zero, neighbours, radius: 2f, random);

            Assert.Less(push.X, 0f);
            Assert.AreEqual(0f, push.Y, Tolerance);
        }

        [Test]
        public void Separation_CloserNeighbourPushesHarder()
        {
            Vector2[] near = { new Vector2(0.5f, 0f) };
            Vector2[] far = { new Vector2(1.5f, 0f) };

            float nearPush = Steering.Separation(Vector2.Zero, near, radius: 2f, random).Length();
            float farPush = Steering.Separation(Vector2.Zero, far, radius: 2f, random).Length();

            Assert.Greater(nearPush, farPush);
        }

        [Test]
        public void Separation_IgnoresNeighbourBeyondRadius()
        {
            Vector2[] neighbours = { new Vector2(2.5f, 0f) };

            Vector2 push = Steering.Separation(Vector2.Zero, neighbours, radius: 2f, random);

            Assert.AreEqual(Vector2.Zero, push);
        }

        [Test]
        public void Separation_NeighbourExactlyOnTop_PushesInSomeDirection()
        {
            Vector2[] neighbours = { Vector2.Zero };

            Vector2 push = Steering.Separation(Vector2.Zero, neighbours, radius: 2f, random);

            Assert.AreEqual(1f, push.Length(), Tolerance);
        }

        [Test]
        public void Chase_MovesTowardTargetAtSpeed()
        {
            Vector2 velocity = Steering.Chase(Vector2.Zero, new Vector2(0f, 10f), Vector2.Zero, separationStrength: 3f, speed: 3f);

            Assert.AreEqual(0f, velocity.X, Tolerance);
            Assert.AreEqual(3f, velocity.Y, Tolerance);
        }

        [Test]
        public void Chase_SeparationBendsDirectionButKeepsSpeed()
        {
            Vector2 velocity = Steering.Chase(Vector2.Zero, new Vector2(0f, 10f), new Vector2(1f, 0f), separationStrength: 3f, speed: 3f);

            Assert.Greater(velocity.X, 0f);
            Assert.AreEqual(3f, velocity.Length(), Tolerance);
        }

        [Test]
        public void Flee_MovesAwayFromThreatAtSpeed()
        {
            Vector2 velocity = Steering.Flee(Vector2.Zero, new Vector2(0f, 10f), Vector2.Zero, separationStrength: 3f, speed: 5f);

            Assert.AreEqual(0f, velocity.X, Tolerance);
            Assert.AreEqual(-5f, velocity.Y, Tolerance);
        }

        [Test]
        public void KeepInside_InsideArena_LeavesVelocityAlone()
        {
            var halfSize = new Vector2(13f, 7f);
            var velocity = new Vector2(3f, 4f);

            Assert.AreEqual(velocity, Steering.KeepInside(new Vector2(0f, 0f), velocity, halfSize));
        }

        [Test]
        public void KeepInside_AtWall_SlidesAlongItAtFullSpeed()
        {
            var halfSize = new Vector2(13f, 7f);

            Vector2 velocity = Steering.KeepInside(new Vector2(13f, 0f), new Vector2(4f, 3f), halfSize);

            Assert.AreEqual(0f, velocity.X, Tolerance);
            Assert.AreEqual(5f, velocity.Y, Tolerance);
        }

        [Test]
        public void KeepInside_InCornerFacingOut_Stops()
        {
            var halfSize = new Vector2(13f, 7f);

            Vector2 velocity = Steering.KeepInside(new Vector2(13f, 7f), new Vector2(4f, 3f), halfSize);

            Assert.AreEqual(Vector2.Zero, velocity);
        }

        [Test]
        public void KeepInside_AtWallMovingInward_IsUnchanged()
        {
            var halfSize = new Vector2(13f, 7f);
            var velocity = new Vector2(-4f, 3f);

            Assert.AreEqual(velocity, Steering.KeepInside(new Vector2(13f, 0f), velocity, halfSize));
        }

        [Test]
        public void Chase_OnTopOfTarget_StandsStillInsteadOfNaN()
        {
            Vector2 velocity = Steering.Chase(Vector2.One, Vector2.One, Vector2.Zero, separationStrength: 3f, speed: 3f);

            Assert.AreEqual(Vector2.Zero, velocity);
        }
    }
}
