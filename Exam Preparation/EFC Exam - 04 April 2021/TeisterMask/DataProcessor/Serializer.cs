namespace TeisterMask.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            StringBuilder output = new StringBuilder();
            StringWriter writer = new StringWriter(output);

            var projectsToExport = context.Projects.Where(t => t.Tasks.Any()).ToArray()
                .Select(p => new ExportProjectXmlDto()
                {
                    TasksCount = p.Tasks.Count(),
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks.Select(t => new ExportTaskDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    }).OrderBy(t => t.Name).ToArray()
                }).OrderByDescending(p => p.Tasks.Count()).ThenBy(p => p.ProjectName).ToArray();



            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlRootAttribute root = new XmlRootAttribute("Projects");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportProjectXmlDto[]), root);  
            
            serializer.Serialize(writer, projectsToExport, namespaces);

            return output.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employeesToExport = context.Employees
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date)).ToArray()
                 .Select(e => new
                 {
                     e.Username,
                     Tasks = e.EmployeesTasks.Where(t => t.Task.OpenDate > date).OrderByDescending(t => t.Task.DueDate).ThenBy(t => t.Task.Name)
                     .Select(t => new
                     {
                         TaskName = t.Task.Name,
                         OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                         DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                         LabelType = t.Task.LabelType.ToString(),
                         ExecutionType = t.Task.ExecutionType.ToString()
                     }).ToArray()
                 }).OrderByDescending(e => e.Tasks.Count()).ThenBy(e => e.Username).Take(10).ToArray();

            return JsonConvert.SerializeObject(employeesToExport, Formatting.Indented);
        }
    }
}