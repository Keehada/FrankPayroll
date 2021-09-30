using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollWebApp.Models
{
    public class Payroll
    {
        [Key]
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PayrollError { get; set; }

    }

    public sealed class PayrollMap : ClassMap<Payroll>
    {
        public PayrollMap()
        {
            Map(m => m.EmployeeId).Ignore();
            Map(m => m.FirstName).Index(1);
            Map(m => m.LastName).Index(2);
            Map(m => m.PayrollError).Index(3);
        }
    }
}
