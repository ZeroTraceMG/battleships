using NUnit.Framework;
using IronTide.Core;

namespace IronTide.Tests.EditMode
{
    public class EventBusTests
    {
        [SetUp]
        public void SetUp() => EventBus.Clear();

        [TearDown]
        public void TearDown() => EventBus.Clear();

        [Test]
        public void Publish_DeliverEventToSubscriber()
        {
            GoldChangedEvent received = default;
            EventBus.Subscribe<GoldChangedEvent>(e => received = e);

            EventBus.Publish(new GoldChangedEvent { PlayerId = "p1", NewTotal = 150, Delta = 50 });

            Assert.AreEqual("p1",  received.PlayerId);
            Assert.AreEqual(150,   received.NewTotal);
            Assert.AreEqual(50,    received.Delta);
        }

        [Test]
        public void Unsubscribe_HandlerNotCalled()
        {
            int callCount = 0;
            void Handler(GoldChangedEvent e) => callCount++;

            EventBus.Subscribe<GoldChangedEvent>(Handler);
            EventBus.Unsubscribe<GoldChangedEvent>(Handler);
            EventBus.Publish(new GoldChangedEvent());

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => EventBus.Publish(new GoldChangedEvent()));
        }

        [Test]
        public void MultipleSubscribers_AllReceiveEvent()
        {
            int count = 0;
            EventBus.Subscribe<ShipDiedEvent>(_ => count++);
            EventBus.Subscribe<ShipDiedEvent>(_ => count++);

            EventBus.Publish(new ShipDiedEvent { VictimId = "ship1", KillerId = "ship2" });

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Publish_DoesNotCrossContaminate_DifferentEventTypes()
        {
            bool goldCalled = false;
            bool shipCalled = false;

            EventBus.Subscribe<GoldChangedEvent>(_ => goldCalled = true);
            EventBus.Subscribe<ShipDiedEvent>(_ => shipCalled = true);

            EventBus.Publish(new GoldChangedEvent());

            Assert.IsTrue(goldCalled);
            Assert.IsFalse(shipCalled);
        }
    }
}
