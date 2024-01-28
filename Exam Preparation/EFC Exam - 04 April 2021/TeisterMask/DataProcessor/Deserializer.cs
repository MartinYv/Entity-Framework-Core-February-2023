// ReSharper disable InconsistentNaming

namespace TeisterMask.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Text;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Globalization;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using Microsoft.VisualBasic;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
    using Newtonsoft.Json;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute root = new XmlRootAttribute("Projects");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProjectXmlDto[]),root);

            ImportProjectXmlDto[] projectsDto = (ImportProjectXmlDto[])serializer.Deserialize(reader);

            HashSet<Task> validTasks = new HashSet<Task>();
            HashSet<Project> validProjects = new HashSet<Project>();

            foreach (var projectDto in projectsDto)
            {

                if (!IsValid(projectDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

               
                DateTime projectStartDate;
                

                bool isProjectStartDateValid = DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectStartDate);

                if (!isProjectStartDateValid)
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? projectDueDate = null;

                if (!String.IsNullOrWhiteSpace(projectDto.DueDate))
                {
                    DateTime dueDateDt;

                    bool isDueDateValid = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDateDt);

                    if (!isDueDateValid)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    projectDueDate = dueDateDt;
                }


                Project project = new Project()
                {
                    Name = projectDto.ProjectName,
                    OpenDate = DateTime.ParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DueDate  = projectDueDate
                };

           

                foreach (var taskDto in projectDto.Tasks)
                {

                    if (!IsValid(taskDto))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }


                    DateTime taskStartDate;
                    DateTime taskDueDate;

                    bool isTaskStartDateValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out taskStartDate);
                    bool isTaskDueDateValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None, out taskDueDate);

                    if (!isTaskStartDateValid || !isTaskDueDateValid)  
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskStartDate > taskDueDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskStartDate < projectStartDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (taskDueDate > projectDueDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;

                    }

                    if (projectDueDate.HasValue && taskDueDate > projectDueDate.Value)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }


                    if (!Enum.IsDefined(typeof(ExecutionType), int.Parse(taskDto.ExecutionType)))
                    {                   
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (!Enum.IsDefined(typeof(LabelType), int.Parse(taskDto.LabelType)))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }



                    Task task = new Task()
                    {
                        Name = taskDto.TaskName,
                        OpenDate = DateTime.ParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ExecutionType = Enum.Parse<ExecutionType>(taskDto.ExecutionType),
                        LabelType = Enum.Parse<LabelType>(taskDto.LabelType)
                    };

                    project.Tasks.Add(task);

                }


                validProjects.Add(project);
                output.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            context.AddRange(validProjects);
            context.SaveChanges();

            return output.ToString().Trim();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            ImportEmployeeJsonDto[] employeesDto = JsonConvert.DeserializeObject<ImportEmployeeJsonDto[]>(jsonString);

            HashSet<Employee> validEntities = new HashSet<Employee>();

            foreach (var employeeDto in employeesDto)
            {
                if (!IsValid(employeeDto))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employee = new Employee()
                {
                    Username = employeeDto.Username,
                    Phone = employeeDto.Phone,
                    Email = employeeDto.Email
                };

              var validTasks = context.Tasks.Select(t =>  t.Id ).ToArray();

                foreach (var taskDtoId in employeeDto.Tasks.Distinct())
                {
                    if (!validTasks.Contains(taskDtoId))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    EmployeeTask employeeTask = new EmployeeTask()
                    {
                        Employee = employee,
                        TaskId = taskDtoId
                    };

                    employee.EmployeesTasks.Add(employeeTask);
                }

                validEntities.Add(employee);
                output.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }

            context.AddRange(validEntities);
            context.SaveChanges();

           return output.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}