using System.Collections.Generic;

namespace IgorekBot.BLL.Models
{
    public class GetProjectTasksResponse
    {
        public int Result { get; set; }

        public IEnumerable<ProjectTask> ProjectTasks { get; set; }

        public string ErrorText { get; set; }
    }
}