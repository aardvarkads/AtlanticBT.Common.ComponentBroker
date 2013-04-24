using System;
using System.Collections.Generic;
using System.Linq;
using AtlanticBT.Common.ComponentBroker.Examples.Model.DataModels;
using AtlanticBT.Common.ComponentBroker.Examples.Model.Repositories;

namespace AtlanticBT.Common.ComponentBroker.Examples.Services
{
    public class BirthdayService
    {
        public List<Employee> GetBirthdays()
        {
            var employeeRepo = ComponentBrokerInstance.RetrieveComponent<IEmployeeRepository>();
            var employees = employeeRepo.GetAllEmployees();

            var now = (DateTime)ComponentBrokerInstance.RetrieveComponent("now");

            return employees.Where(employee => employee.BirthDay.Month == now.Month 
                                                && employee.BirthDay.Day == now.Day).ToList();
        }
    }
}
