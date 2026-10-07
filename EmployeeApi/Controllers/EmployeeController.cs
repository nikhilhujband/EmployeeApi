using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                Name = "Rahul Sharma",
                PhoneNumber = "9876543210",
                Email = "rahul.sharma@gmail.com",
                Department = "IT",
                Salary = 65000
            },
            new Employee
            {
                Id = 2,
                Name = "Priya Patil",
                PhoneNumber = "9876543211",
                Email = "priya.patil@gmail.com",
                Department = "HR",
                Salary = 55000
            }
        };

        // GET: api/Employee
        [HttpGet]
        public IActionResult GetEmployees()
        {
            return Ok(employees);
        }

        // POST: api/Employee
        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            employee.Id = employees.Any()
                ? employees.Max(e => e.Id) + 1
                : 1;

            employees.Add(employee);

            return Ok(employee);
        }

        // PUT: api/Employee/1
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, Employee updatedEmployee)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            employee.Name = updatedEmployee.Name;
            employee.PhoneNumber = updatedEmployee.PhoneNumber;
            employee.Email = updatedEmployee.Email;
            employee.Department = updatedEmployee.Department;
            employee.Salary = updatedEmployee.Salary;

            return Ok(employee);
        }

        // DELETE: api/Employee/1
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            employees.Remove(employee);

            return Ok("Employee deleted successfully");
        }
    }
}