using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class SessionRecordTests
    {
        [Test]
        public void FirstScore_IsNewRecord()
        {
            var record = new SessionRecord();

            Assert.IsTrue(record.Submit(120));
            Assert.AreEqual(120, record.BestScore);
        }

        [Test]
        public void LowerOrEqualScore_IsNotRecord()
        {
            var record = new SessionRecord();
            record.Submit(500);

            Assert.IsFalse(record.Submit(400));
            Assert.IsFalse(record.Submit(500));
            Assert.AreEqual(500, record.BestScore);
        }

        [Test]
        public void HigherScore_BeatsRecord()
        {
            var record = new SessionRecord();
            record.Submit(500);

            Assert.IsTrue(record.Submit(510));
            Assert.AreEqual(510, record.BestScore);
        }
    }
}
