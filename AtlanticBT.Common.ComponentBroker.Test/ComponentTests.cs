using System;
using AtlanticBT.Common.ComponentBroker.Examples.Factories;
using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;
using AtlanticBT.Common.ComponentBroker.Examples.Services;
using NUnit.Framework;

namespace AtlanticBT.Common.ComponentBroker.Test
{
    [TestFixture]
    public class ComponentTests
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

        #region TEST REGISTER COMPONENT

        /// <summary>
        /// Verify that a component can be registered for
        /// a given type.
        /// </summary>
        [Test]
        public void TestRegisterComponentByType()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(employeeRepository);
            var registeredComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>();
            Assert.AreSame(employeeRepository, registeredComponent);
        }

        /// <summary>
        /// Verify that a component can be registered for
        /// a given key.
        /// </summary>
        [Test]
        public void TestRegisterComponentByKey()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent(typeof(IEmployeeRepository).AssemblyQualifiedName, employeeRepository);
            var registeredComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>();
            Assert.AreSame(employeeRepository, registeredComponent);
        }

        /// <summary>
        /// Verify that registercomponent throws an argument exception
        /// when a null component is provided.
        /// </summary>
        [Test]
        public void TestRegisterNullComponent()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterComponent(typeof(IEmployeeRepository).AssemblyQualifiedName, null));
            Assert.That(ex.ParamName, Is.EqualTo("component"));
        }

        /// <summary>
        /// Verify that registercomponent throws an argument exception
        /// when a null key is provided.
        /// </summary>
        [Test]
        public void TestRegisterComponentWithNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterComponent(null, new object()));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that registercomponent throws an argument exception
        /// when an empty key is provided.
        /// </summary>
        [Test]
        public void TestRegisterComponentWithEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RegisterComponent(string.Empty, new object()));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that the component overwrites the component previously
        /// registered to the same key.
        /// </summary>
        [Test]
        public void TestRegisterComponentWhenOneExists()
        {
            var firstEmployeeRepository = new EmployeeRepository();
            var secondEmployeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(firstEmployeeRepository);
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(secondEmployeeRepository);
            var registeredComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>();
            Assert.AreSame(secondEmployeeRepository, registeredComponent);
        }

        #endregion

        #region TEST HAS COMPONENT

        /// <summary>
        /// Verify that hascomponent returns true
        /// if a component is registered for the given type.
        /// </summary>
        [Test]
        public void TestHasComponentByTypeExists()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(employeeRepository);
            Assert.IsTrue(ComponentBrokerInstance.HasComponent<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that hascomponent returns false 
        /// if no component is registered for the given type.
        /// </summary>
        [Test]
        public void TestHasComponentByTypeDoesNotExist()
        {
            Assert.IsFalse(ComponentBrokerInstance.HasComponent<IEmployeeRepository>());
        }

        /// <summary>
        /// Verify that hascomponent returns true if 
        /// a component is registered for a given key.
        /// </summary>
        [Test]
        public void TestHasComponentByKeyExists()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent(typeof(IEmployeeRepository).AssemblyQualifiedName, employeeRepository);
            Assert.IsTrue(ComponentBrokerInstance.HasComponent(typeof(IEmployeeRepository).AssemblyQualifiedName));
        }

        /// <summary>
        /// Verify that hascomponent returns false if no
        /// component is registered for a given key.
        /// </summary>
        [Test]
        public void TestHasComponentByKeyDoesNotExist()
        {
            Assert.IsFalse(ComponentBrokerInstance.HasComponent(typeof(IEmployeeRepository).AssemblyQualifiedName));
        }

        /// <summary>
        /// Verify that hascomponent throws an argument exception 
        /// if a null key is provided.
        /// </summary>
        [Test]
        public void TestHasComponentByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasComponent(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that a hascomponent throws an argument exception 
        /// if an empty key is provided.
        /// </summary>
        [Test]
        public void TestHasComponentByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.HasComponent(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #region TEST RETRIEVE COMPONENT

        #region RetrieveNewComponent

        /// <summary>
        /// Verify that a new component can be created if one exists.
        /// </summary>
        [Test]
        public void TestRetrieveNewComponentByTypeExists()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(employeeRepository);
            var retrievedNewComponent = ComponentBrokerInstance.RetrieveNewComponent<IEmployeeRepository>();
            Assert.AreNotSame(employeeRepository, retrievedNewComponent);
        }

        /// <summary>
        /// Verify that a new component can be created if non already exists.
        /// </summary>
        [Test]
        public void TestRetrieveNewComponentByTypeDoesNotExist()
        {
            var retrievedNewComponent = ComponentBrokerInstance.RetrieveNewComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), retrievedNewComponent);
        }

        /// <summary>
        /// Verify that a new component can be retrieved and not registered.
        /// </summary>
        [Test]
        public void TestRetrieveNewComponentByTypeNoRegister()
        {
            var retrievedNewComponent = ComponentBrokerInstance.RetrieveNewComponent<IEmployeeRepository>(false);
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), retrievedNewComponent);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), retrievedComponent);
            Assert.AreNotSame(retrievedNewComponent, retrievedComponent);
        }

        #endregion

        #region RetrieveComponent

        /// <summary>
        /// Verify that a component can be retrieved by its type
        /// if it is registered.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByTypeExists()
        {
            var employeeRepository = new EmployeeRepository();
            ComponentBrokerInstance.RegisterComponent<IEmployeeRepository>(employeeRepository);
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>();
            Assert.AreSame(employeeRepository, retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be retrieved by its type
        /// if it is not registered.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByTypeDoesNotExist()
        {
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be retrieved by its type 
        /// and that it is not registered.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByTypeNoRegister()
        {
            var firstRetrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            var secondRetrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), firstRetrievedComponent);
            Assert.IsInstanceOf(typeof(IEmployeeRepository), secondRetrievedComponent);
            Assert.AreNotSame(firstRetrievedComponent, secondRetrievedComponent);
        }

        #endregion

        #region CreateComponent via RetrieveComponent

        /// <summary>
        /// Verify that a component can be created using a factory with constructor arguments.
        /// </summary>
        [Test]
        public void TestCreateComponentByFactoryWithArgs()
        {
            ComponentBrokerInstance.RegisterFactory<IUserService, UserServiceFactory>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IUserService>(false, new EmployeeRepository());
            Assert.IsInstanceOf(typeof(UserService), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be created using constructor arguments.
        /// </summary>
        [Test]
        public void TestCreateComponentWithArgs()
        {
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IUserService>(false, new EmployeeRepository());
            Assert.IsInstanceOf(typeof(UserService), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component cannot be created from an abstract class.
        /// </summary>
        [Test]
        public void TestCreateComponentAbstractThrowsError()
        {
            Assert.Throws<ComponentBrokerException>(() => ComponentBrokerInstance.RetrieveComponent<PeopleService>(false));
        }

        /// <summary>
        /// Verify that a component can be created based off of a concrete class type.
        /// </summary>
        [Test]
        public void TestCreateComponentConcrete()
        {
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<EmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(EmployeeRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be created using the standard naming convention.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByNamingConvention()
        {
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(EmployeeRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be created using an alternate naming convention.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByAlternateNamingConvention()
        {
            ComponentBrokerInstance.InterfaceRegexMask = "Interface$";
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<ManagerRepositoryInterface>(false);
            Assert.IsInstanceOf(typeof(ManagerRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be created using a type association.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByTypeAssociation()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, ManagerRepository>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(ManagerRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that a component can be created using a factory.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByFactory()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(UserRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that factories take precedence over type associations.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByFactoryPrecedenceOverTypeAssocation()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, ManagerRepository>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(UserRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that factories take precedence over naming convention.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByFactoryPrecedenceOverNamingConvention()
        {
            ComponentBrokerInstance.RegisterFactory<IEmployeeRepository, EmployeeRepositoryFactory>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(UserRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that type associations take precedence over naming convention.
        /// </summary>
        [Test]
        public void TestCreateComponentInterfaceByTypeAssociationPrecedenceOverNamingConvention()
        {
            ComponentBrokerInstance.RegisterType<IEmployeeRepository, ManagerRepository>();
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(false);
            Assert.IsInstanceOf(typeof(ManagerRepository), retrievedComponent);
        }

        #endregion

        #region RetrieveComponent Standard
        
        /// <summary>
        /// Verify that a component can be retrieved as its type 
        /// by its key if it is registered.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByKeyAsTypeExists()
        {
            var key = typeof (IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterComponent(key, new EmployeeRepository());
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(key);
            Assert.IsInstanceOf(typeof(EmployeeRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that if a component is retrieved by its key
        /// and it does not exist that a relevant component broker exception
        /// is thrown.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByKeyAsTypeDoesNotExist()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            Assert.Throws<ComponentBrokerException>(() => ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>(key));
        }

        /// <summary>
        /// Verify that a component can be retrived as an object
        /// by its key if it is registered.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByKeyExists()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            ComponentBrokerInstance.RegisterComponent(key, new EmployeeRepository());
            var retrievedComponent = ComponentBrokerInstance.RetrieveComponent(key);
            Assert.IsInstanceOf(typeof(EmployeeRepository), retrievedComponent);
        }

        /// <summary>
        /// Verify that if a component does not exist that 
        /// a relevant component broker exception is thrown.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByKeyDoesNotExist()
        {
            var key = typeof(IEmployeeRepository).AssemblyQualifiedName;
            Assert.Throws<ComponentBrokerException>(() => ComponentBrokerInstance.RetrieveComponent(key));
        }

        /// <summary>
        /// Verify that if a component is retrieved using a null key
        /// taht an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByNullKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RetrieveComponent(null));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        /// <summary>
        /// Verify that if a component is retrieved using an empty string
        /// as a key that an argument exception is thrown.
        /// </summary>
        [Test]
        public void TestRetrieveComponentByEmptyKey()
        {
            var ex = Assert.Throws<ArgumentException>(() => ComponentBrokerInstance.RetrieveComponent(string.Empty));
            Assert.That(ex.ParamName, Is.EqualTo("key"));
        }

        #endregion

        #endregion
    }
}
