using NUnit.Framework;
using IronTide.Core;

namespace IronTide.Tests.EditMode
{
    public class ServiceLocatorTests
    {
        [SetUp]
        public void SetUp() => ServiceLocator.Clear();

        [TearDown]
        public void TearDown() => ServiceLocator.Clear();

        [Test]
        public void Register_And_Get_ReturnsCorrectInstance()
        {
            var stub = new StubService { Value = 42 };
            ServiceLocator.Register<StubService>(stub);

            var result = ServiceLocator.Get<StubService>();

            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void Get_Unregistered_ReturnsNull()
        {
            var result = ServiceLocator.Get<StubService>();
            Assert.IsNull(result);
        }

        [Test]
        public void Has_ReturnsTrueWhenRegistered()
        {
            ServiceLocator.Register(new StubService());
            Assert.IsTrue(ServiceLocator.Has<StubService>());
        }

        [Test]
        public void Has_ReturnsFalseWhenNotRegistered()
        {
            Assert.IsFalse(ServiceLocator.Has<StubService>());
        }

        [Test]
        public void Unregister_RemovesService()
        {
            ServiceLocator.Register(new StubService());
            ServiceLocator.Unregister<StubService>();
            Assert.IsFalse(ServiceLocator.Has<StubService>());
        }

        private class StubService { public int Value; }
    }
}
