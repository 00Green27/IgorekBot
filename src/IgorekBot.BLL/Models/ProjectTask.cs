using System;

namespace IgorekBot.BLL.Models
{
    [Serializable]
    public class ProjectTask
    {
        public string ProjectNo { get; set; }
        public string TaskNo { get; set; }
        public string TaskDescription { get; set; }
        public string AssignmentCode { get; set; }
        public string Description { get; set; }
    }
}