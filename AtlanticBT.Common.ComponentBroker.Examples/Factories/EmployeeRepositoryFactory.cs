using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;

namespace AtlanticBT.Common.ComponentBroker.Examples.Factories
{
    public class EmployeeRepositoryFactory : IComponentFactory<IEmployeeRepository>
    {
        public IEmployeeRepository Create()
        {
            return new UserRepository();
        }
    }
}
