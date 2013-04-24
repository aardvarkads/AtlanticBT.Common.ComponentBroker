using System.Collections.Generic;
using AtlanticBT.Common.ComponentBroker.Examples.Model.DataModels;

namespace AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories
{
    public class ManagerRepository : IEmployeeRepository, ManagerRepositoryInterface
    {
        public IList<Employee> GetAllEmployees()
        {
            return new List<Employee>();
        }
    }
}
