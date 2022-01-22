using NUnit.Framework;
using UnitTests.Test;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Block;
using DependencyInjectionContainer.Helper;

namespace UnitTests.Helper
{
    [TestFixture]
    public class ValidatorTest
    {
        private DependenciesConfiguration Configuration;

        [SetUp]
        public void Init()
        {
            Configuration = new DependenciesConfiguration();
        }

        [Test]
        public void TwoImplementationsWithoutNestedInterfaceValidTest()
        {
            Configuration.Register<ICommand, MyCommand1>(TTL.Singleton);
            Configuration.Register<ICommand, MyCommand2>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsTrue(validator.IsValid());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceValidTest()
        {
            Configuration.Register<IRepository, Repository>(TTL.InstancePerDependency);
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsTrue(validator.IsValid());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceInValidTest()
        {
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsFalse(validator.IsValid());
        }
    }
}
