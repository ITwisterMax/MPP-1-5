using System;
using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;
using UnitTests.Test;
using DependencyInjectionContainer.Api.Parameters;
using DependencyInjectionContainer.Block;
using DependencyInjectionContainer.Model;

namespace UnitTests.Configuration
{
    [TestFixture]
    public class DependenciesConfigurationTest
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
            var expected = new Dictionary<Type, List<Implementation>>()
            {
                {
                    typeof(ICommand),
                    new List<Implementation>()
                    {
                        new Implementation(typeof(MyCommand1), TTL.Singleton, ServiceImplementations.None),
                        new Implementation(typeof(MyCommand2), TTL.InstancePerDependency, ServiceImplementations.None)
                    }
                }
            };

            Configuration.Register<ICommand, MyCommand1>(TTL.Singleton);
            Configuration.Register<ICommand, MyCommand2>(TTL.InstancePerDependency);

            var expectedJson = JsonConvert.SerializeObject(Configuration.Dependencies);
            var resultJson = JsonConvert.SerializeObject(expected);

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceTest()
        {
            var expected = new Dictionary<Type, List<Implementation>>()
            {
                {
                    typeof(IChat),
                    new List<Implementation>()
                    {
                        new Implementation(typeof(TelegramChat), TTL.Singleton, ServiceImplementations.None),
                        new Implementation(typeof(VkChat), TTL.InstancePerDependency, ServiceImplementations.None)
                    }
                }
            };

            Configuration.Register<IChat, TelegramChat>(TTL.Singleton);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency);

            var expectedJson = JsonConvert.SerializeObject(Configuration.Dependencies);
            var resultJson = JsonConvert.SerializeObject(expected);

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void OneImplementationWithCustomAttribute()
        {
            var expected = new Dictionary<Type, List<Implementation>>()
            {
                {
                    typeof(IFirstChat),
                    new List<Implementation>()
                    {
                        new Implementation(typeof(FirstChat), TTL.InstancePerDependency, ServiceImplementations.None)
                    }
                }
            };

            Configuration.Register<IFirstChat, FirstChat>(TTL.InstancePerDependency);

            var expectedJson = JsonConvert.SerializeObject(Configuration.Dependencies);
            var resultJson = JsonConvert.SerializeObject(expected);

            Assert.AreEqual(expectedJson, resultJson);
        }
    }
}
