using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
   public class StartUp
    {
        static void Main(string[] args)
        {

            SoftUniContext dbContext = new SoftUniContext();

            string result = DeleteProjectById(dbContext);
            Console.WriteLine(result);

        }
           public static string GetEmployeesFullInformation(SoftUniContext context)
           {

            StringBuilder sb = new StringBuilder();

            Employee[] allEmployees = context.Employees.OrderBy(e => e.EmployeeId).ToArray();

            foreach (var employee in allEmployees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString();

           }


        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {

            StringBuilder sb = new StringBuilder();

           var employees = context.Employees.Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                 })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();


            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.Where(e => e.Salary > 50000).OrderBy(e=>e.FirstName).ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address address = new Address();
            address.TownId = 4;
            address.AddressText = "Vitoshka 15";

            context.Addresses.Add(address);

            Employee employee = context.Employees.First(e => e.LastName == "Nakov");
            employee.Address = address;

            context.SaveChangesAsync();


            var allEmployeesAddresses = context.Employees.OrderByDescending(e => e.AddressId).Take(10).
                Select(e=> new
                {
                    e.Address.AddressText
                });

            foreach (var e in allEmployeesAddresses)
            {
                sb.AppendLine(e.AddressText);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {

            StringBuilder sb = new StringBuilder();

            var employees = context.Employees.
                Where(e => e.EmployeesProjects.Any(p => p.Project.StartDate.Year >= 2001 && p.Project.StartDate.Year <= 2003)).
                Take(10).
                Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                   MenagerFirstName = e.Manager.FirstName,
                    MenagerLastName = e.Manager.LastName,
                    AllProjects = e.EmployeesProjects.Select(p=> new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = p.Project.EndDate.HasValue ? p.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                    }).ToArray()
                }).ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.MenagerFirstName} {e.MenagerLastName}");

                foreach (var p in e.AllProjects)
                {
                    
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");

                }

            }

            return sb.ToString();
        }


        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            // var addresses = context.Addresses.
            //     OrderByDescending(a => a.Employees.Count()).
            //     OrderBy(a => a.Town.Name).
            //     OrderBy(a => a.AddressText).
            //     Take(10).
            //     ToArray();

            var addresses = context.Addresses.Select(a => new
            {
                employeesCount = a.Employees.Count(),
                townName = a.Town.Name,
                addressText = a.AddressText
            }).OrderByDescending(a => a.employeesCount).ThenBy(t => t.townName).ThenBy(at => at.addressText).Take(10).ToArray();


            foreach (var a in addresses)
            {
                output.AppendLine($"{a.addressText}, {a.townName} - {a.employeesCount} employees");
            }

            return output.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employeeId147 = context.Employees.Where(e => e.EmployeeId == 147).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects.Select(p => new
                {
                    ProjectName = p.Project.Name
                })
            }).ToArray();

            

            foreach (var employee in employeeId147)
            {
                output.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

                foreach (var p in employee.Projects.OrderBy(x=>x.ProjectName))
                {
                    output.AppendLine(p.ProjectName);
                }
            }


            return output.ToString();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();


            var departments = context.Departments.Where(d=>d.Employees.Count() > 5).OrderBy(ec=>ec.Employees.Count()).ThenBy(d=>d.Name).
                Select(d=> new
                {
                    
                    DepartmentName = d.Name,
                    MenagerFirstName = d.Manager.FirstName,
                    Employees = d.Employees.Select(e=> new
                                                     {
                                                         EFirstName = e.FirstName,
                                                         ELastName = e.LastName,
                                                         EJobTitle = e.JobTitle
                                                     }
                                                   )
               }).ToArray();

            foreach (var d in departments)
            {
                output.AppendLine($"{d.DepartmentName} - {d.MenagerFirstName}");

                foreach (var e in d.Employees.OrderBy(e=>e.EFirstName).ThenBy(e=>e.ELastName))
                {
                    output.AppendLine($"{e.EFirstName} {e.ELastName} - {e.EJobTitle}");
                }
            }

            return output.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10).OrderBy(p => p.Name)
                .Select(x => new
                                {
                                    x.Name,
                                    x.Description,
                                    x.StartDate

                                }
                        ).ToArray();


            foreach (var p in projects)
            {
                output.AppendLine(p.Name);
                output.AppendLine(p.Description);
                output.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return output.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();

            var employeesToIncreaceSalary = context.Employees.Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design" ||
            e.Department.Name == "Marketing" || e.Department.Name == "Information Services").OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToArray();

            foreach (var e in employeesToIncreaceSalary)
            {
                e.Salary += e.Salary * (decimal)0.12;
                output.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }


            return output.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();


            var empoyees = context.Employees.Where(x => x.FirstName.ToLower().StartsWith("sa"))
                                            .OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToArray();


            foreach (var e in empoyees)
            {
                output.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return output.ToString().TrimEnd();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder output = new StringBuilder();


            Project projectToDelete = context.Projects.FirstOrDefault(x => x.ProjectId == 2); // 0/100

         

            context.Projects.Remove(projectToDelete);

            var projectsForOutput = context.EmployeesProjects.Select(x=>x.Project.Name).Take(10).ToArray();

            foreach (var p in projectsForOutput)
            {
                output.AppendLine($"{p.ToString()}");
            }

            return output.ToString().TrimEnd();

        }
    }
}
