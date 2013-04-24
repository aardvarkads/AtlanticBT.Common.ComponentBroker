using System;

namespace AtlanticBT.Common.ComponentBroker.Examples.Model.DataModels
{
    public class Employee
    {
        public Guid Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
