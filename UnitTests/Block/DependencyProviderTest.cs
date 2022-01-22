using NUnit.Framework;
using UnitTests.Test;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Block;

namespace UnitTests.Block
{
    [TestFixture]
    public class DependenciesProviderTest
    {
        private DependenciesConfiguration Configuration;

        [SetUp]
        public void Init()
        {
            Configuration = new DependenciesConfiguration();
        }

        [Test]
        public void TwoImplementationsWithoutNestedInterfaceTest()
        {
            Configuration.Register<ICommand, MyCommand1>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<ICommand, MyCommand2>(TTL.InstancePerDependency, ServiceImplementations.Second);

            var provider = new DependencyProvider(Configuration);

            int expected = 1;
            var actual = provider.Resolve<ICommand>(ServiceImplementations.First);

            Assert.AreEqual(expected, actual.TestCommand());

            expected = 2;
            actual = provider.Resolve<ICommand>(ServiceImplementations.Second);

            Assert.AreEqual(expected, actual.TestCommand());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceTest()
        {
            Configuration.Register<IRepository, Repository>(TTL.InstancePerDependency);
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency, ServiceImplementations.Second);

            var provider = new DependencyProvider(Configuration);

            string expected = "Telegram";
            var actual = provider.Resolve<IChat>(ServiceImplementations.First);

            Assert.AreEqual(expected, actual.SendMessage());

            expected = "Vk";
            actual = provider.Resolve<IChat>(ServiceImplementations.Second);

            Assert.AreEqual(expected, actual.SendMessage());
        }

        [Test]
        public void OneImplementationWithCustomAttribute()
        {
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency, ServiceImplementations.Second);
            Configuration.Register<IRepository, Repository>(TTL.Singleton);
            Configuration.Register<IFirstChat, FirstChat>(TTL.InstancePerDependency);

            var provider = new DependencyProvider(Configuration);

            string expected = "UnitTests.Test.TelegramChat";
            var actual = provider.Resolve<IFirstChat>();

            Assert.IsInstanceOf(typeof(TelegramChat), actual.GetChat());
            Assert.AreEqual(expected, actual.GetChat().ToString());
        }
    }
}
