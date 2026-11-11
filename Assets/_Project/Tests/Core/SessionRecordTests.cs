using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class SessionRecordTests
    {
        [Test]
        public void FirstWave_IsNewRecord()
        {
            var record = new SessionRecord();

            Assert.IsTrue(record.Submit(3));
            Assert.AreEqual(3, record.BestWave);
        }

        [Test]
        public void LowerOrEqualWave_IsNotRecord()
        {
            var record = new SessionRecord();
            record.Submit(5);

            Assert.IsFalse(record.Submit(4));
            Assert.IsFalse(record.Submit(5));
            Assert.AreEqual(5, record.BestWave);
        }

        [Test]
        public void HigherWave_BeatsRecord()
        {
            var record = new SessionRecord();
            record.Submit(5);

            Assert.IsTrue(record.Submit(6));
            Assert.AreEqual(6, record.BestWave);
        }
    }
}
