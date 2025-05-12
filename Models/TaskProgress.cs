using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskProgress
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public TaskModel Taskss { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        public DateTime DueDate { get; set; }
        [Required]
        public string TaskStatus { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
