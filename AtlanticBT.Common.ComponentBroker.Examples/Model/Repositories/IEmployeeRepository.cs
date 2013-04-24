using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtlanticBT.Common.ComponentBroker.Examples.Model.DataModels;

namespace AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories
{
    public interface IEmployeeRepository
    {
        IList<Employee> GetAllEmployees();
    }
}
