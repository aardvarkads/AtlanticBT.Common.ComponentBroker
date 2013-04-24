using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;
using AtlanticBT.Common.ComponentBroker.Examples.Services;

namespace AtlanticBT.Common.ComponentBroker.Examples.Factories
{
    public class UserServiceFactory : IComponentFactory<IUserService>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UserServiceFactory(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IUserService Create()
        {
            return new UserService(_employeeRepository);
        }
    }
}
