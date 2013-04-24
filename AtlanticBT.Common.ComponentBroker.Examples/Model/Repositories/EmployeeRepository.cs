using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtlanticBT.Common.ComponentBroker.Examples.Model.DataModels;

namespace AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public IList<Employee> GetAllEmployees()
        {
            return new List<Employee>()
                       {
                           new Employee()
                               {
                                   Id = Guid.NewGuid(),
                                   FirstName = "Bob",
                                   LastName = "Cratchet",
                                   BirthDay = new DateTime(1985, 12, 3)
                               },
                           new Employee()
                               {
                                   Id = Guid.NewGuid(),
                                   FirstName = "George",
                                   LastName = "Passat",
                                   BirthDay = new DateTime(1965, 7, 8)
                               }
                       };
        }
    }
}
