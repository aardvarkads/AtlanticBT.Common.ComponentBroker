using System;
using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;
using AtlanticBT.Common.ComponentBroker.Examples.Services;
using NUnit.Framework;

namespace AtlanticBT.Common.ComponentBroker.Test
{
    [TestFixture]
    public class TypeTests
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
        /// Verify that a Type can be registered by type.
        /// </summary>
        [Test]
        public void TestRegisterComponentTypeByType()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, EmployeeRepository>();
            Assert.IsTrue(ComponentBrokerInstance.HasType<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that a Type can be registered by a key.
        /// </summary>
        [Test]
        public void TestRegisterComponentTypeTypeByKey()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterType<EmployeeRepository>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasType(key));
        }

        /// <summary>
        /// Verify that registering a Type with a null key 
        /// throws an argument exception.
        /// </summary>
        [Test]
        public void TestRegisterComponentTypeByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterType<EmployeeRepository>(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that registering a Type with an empty key 
        /// throws an argument exception.
        /// </summary>
        [Test]
        public void TestRegisterComponentTypeByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterType<EmployeeRepository>(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #region HAS Type

        /// <summary>
        /// Verify that if a Type is registered to a type then 
        /// it returns true.
        /// </summary>
        [Test]
        public void TestHasTypeByTypeExists()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, EmployeeRepository>();
            Assert.IsTrue(ComponentBrokerInstance.HasType<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that if a Type is not registered for a type then
        /// it returns false.
        /// </summary>
        [Test]
        public void TestHasTypeByTypeDoesNotExist()
        {
            Assert.IsFalse(ComponentBrokerInstance.HasType<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that if a Type is registered for a key 
        /// then it returns true.
        /// </summary>
        [Test]
        public void TestHasTypeByKeyExists()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterType<EmployeeRepository>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasType(key));
        }


        /// <summary>
        /// Verify that if a Type is not registered for a key 
        /// then it returns false.
        /// </summary>
        [Test]
        public void TestHasTypeByKeyDoesNotExist()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            Assert.IsFalse(ComponentBrokerInstance.HasType(key));
        }

        /// <summary>
        /// Verify that if a null key is provided 
        /// then an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestHasTypeNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasType(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that if an empty key is provided 
        /// then an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestHasTypeEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasType(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #region UNREGISTER Type

        /// <summary>
        /// Verify that a Type can be unregistered by
        /// the type it was registered to.
        /// </summary>
        [Test]
        public void TestUnregisterTypeByType()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, EmployeeRepository>();
            Assert.IsTrue(ComponentBrokerInstance.HasType<IEmployeeRepository>());
            ComponentBrokerInstance.UnregisterType<IEmployeeRepository>();
            Assert.IsFalse(ComponentBrokerInstance.HasType<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that a Type can be unregistered by 
        /// the key it was registered to.
        /// </summary>
        [Test]
        public void TestUnregisterTypeByKey()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterType<EmployeeRepository>(key);
            Assert.IsTrue(ComponentBrokerInstance.HasType(key));
            ComponentBrokerInstance.UnregisterType(key);
            Assert.IsFalse(ComponentBrokerInstance.HasType(key));
        }

        /// <summary>
        /// Verify that an argument exception is thrown 
        /// when a null key is provided.
        /// </summary>
        [Test]
        public void TestUnregisterTypeByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.UnregisterType(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that an argument exception is thrown 
        /// when an empty key is provided.
        /// </summary>
        [Test]
        public void TestUnregisterTypeByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.UnregisterType(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that all registered type associations 
        /// can be removed.
        /// </summary>
        [Test]
        public void TestUnregisterAllTypes()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, EmployeeRepository>();
            ComponentBrokerInstance.RegisterType<IUserService, UserService>();
            Assert.IsTrue(ComponentBrokerInstance.HasType<IEmployeeRepository>());
            Assert.IsTrue(ComponentBrokerInstance.HasType<IUserService>());
            ComponentBrokerInstance.UnregisterAllTypes();
            Assert.IsFalse(ComponentBrokerInstance.HasType<IEmployeeRepository>());
            Assert.IsFalse(ComponentBrokerInstance.HasType<IUserService>());
        }

        #endregion
    }
}
