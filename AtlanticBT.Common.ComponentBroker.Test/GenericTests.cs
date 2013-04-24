using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtlanticBT.Common.ComponentBroker.Examples.Factories;
using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;
using NUnit.Framework;

namespace AtlanticBT.Common.ComponentBroker.Test
{
    [TestFixture]
    public class GenericTests
    {
        /// <summary>
        /// Ensure that the component broker is set to its default state
        /// and initialize the employee repository mock.
        /// </summary>
        [SetUp]
        public void TestInit()
        {
            ComponentBrokerInstance.Reset();
        }

        /// <summary>
        /// Reset the component broker back to its original state after each test.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            ComponentBrokerInstance.Reset();
        }

        /// <summary>
        /// Verify that all can be reset.
        /// </summary>
        [Test]
        public void TestReset()
        {
            // Factory
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());

            // Type Association
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, EmployeeRepository>();
            Assert.IsTrue(ComponentBrokerInstance.HasType<IEmployeeRepository>());

            // Concrete Component
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(new EmployeeRepository());
            Assert.IsTrue(ComponentBrokerInstance.HasComponent<IEmployeeRepository>());

            // Naming Convention
            const string mask = "Interface$";
            Assert.AreNotEqual(ComponentBrokerInstance.InterfaceRegexMask, mask);
            ComponentBrokerInstance.InterfaceRegexMask = mask;
            Assert.AreEqual(ComponentBrokerInstance.InterfaceRegexMask, mask);

            // reset
            ComponentBrokerInstance.Reset();
            
            // verify
            Assert.IsFalse(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
            Assert.IsFalse(ComponentBrokerInstance.HasType<IEmployeeRepository>());
            Assert.IsFalse(ComponentBrokerInstance.HasComponent<IEmployeeRepository>());
            Assert.AreEqual(ComponentBrokerInstance.InterfaceRegexMask, "^I");
        }
    }
}
