using System;
using AtlanticBT.Common.ComponentBroker.Examples.Factories;
using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;
using AtlanticBT.Common.ComponentBroker.Examples.Services;
using NUnit.Framework;

namespace AtlanticBT.Common.ComponentBroker.Test
{
    [TestFixture]
    public class FactoryTests
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

        #region REGISTER

        /// <summary>
        /// Verify that a factory can be registered by type.
        /// </summary>
        [Test]
        public void TestRegisterComponentFactoryByType()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that a factory can be registered by a key.
        /// </summary>
        [Test]
        public void TestRegisterComponentFactoryTypeByKey()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterFactory<EmployeeRepositoryFactory>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasFactory(key));
        }

        /// <summary>
        /// Verify that registering a factory with a null key 
        /// throws an argument exception.
        /// </summary>
        [Test]
        public void TestRegisterComponentFactoryByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterFactory<EmployeeRepositoryFactory>(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that registering a factory with an empty key 
        /// throws an argument exception.
        /// </summary>
        [Test]
        public void TestRegisterComponentFactoryByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterFactory<EmployeeRepositoryFactory>(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #region HAS FACTORY

        /// <summary>
        /// Verify that if a factory is registered to a type then 
        /// it returns true.
        /// </summary>
        [Test]
        public void TestHasFactoryByTypeExists()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that if a factory is not registered for a type then
        /// it returns false.
        /// </summary>
        [Test]
        public void TestHasFactoryByTypeDoesNotExist()
        {
            Assert.IsFalse(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that if a factory is registered for a key 
        /// then it returns true.
        /// </summary>
        [Test]
        public void TestHasFactoryByKeyExists()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterFactory<EmployeeRepositoryFactory>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasFactory(key));
        }


        /// <summary>
        /// Verify that if a factory is not registered for a key 
        /// then it returns false.
        /// </summary>
        [Test]
        public void TestHasFactoryByKeyDoesNotExist()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            Assert.IsFalse(ComponentBrokerInstance.HasFactory(key));
        }

        /// <summary>
        /// Verify that if a null key is provided 
        /// then an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestHasFactoryNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasFactory(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that if an empty key is provided 
        /// then an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestHasFactoryEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasFactory(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #region UNREGISTER FACTORY

        /// <summary>
        /// Verify that a factory can be unregistered by
        /// the type it was registered to.
        /// </summary>
        [Test]
        public void TestUnregisterFactoryByType()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
            ComponentBrokerInstance.UnregisterFactory<IEmployeeRepository>();
            Assert.IsFalse(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that a factory can be unregistered by 
        /// the key it was registered to.
        /// </summary>
        [Test]
        public void TestUnregisterFactoryByKey()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterFactory<EmployeeRepositoryFactory>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasFactory(key));
            ComponentBrokerInstance.UnregisterFactory(key);
            Assert.IsFalse(ComponentBrokerInstance.HasFactory(key));
        }

        /// <summary>
        /// Verify that an argument exception is thrown 
        /// when a null key is provided.
        /// </summary>
        [Test]
        public void TestUnregisterFactoryByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.UnregisterFactory(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that an argument exception is thrown 
        /// when an empty key is provided.
        /// </summary>
        [Test]
        public void TestUnregisterFactoryByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.UnregisterFactory(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that all registered factories 
        /// can be removed.
        /// </summary>
        [Test]
        public void TestUnregisterAllFactories()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            ComponentBrokerInstance.RegisterFactory<IUserService, UserServiceFactory>();
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
            Assert.IsTrue(ComponentBrokerInstance.HasFactory<IUserService>());
            ComponentBrokerInstance.UnregisterAllFactories();
            Assert.IsFalse(ComponentBrokerInstance.HasFactory<IEmployeeRepository>());
            Assert.IsFalse(ComponentBrokerInstance.HasFactory<IUserService>());
        }

        #endregion
    }
}
