using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;

namespace AtlanticBT.Common.ComponentBroker.Examples.Services
{
    public class UserService : IUserService
    {
        public UserService(IEmployeeRepository employeeRepository)
        {
            
        }
    }
}
