using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string Priority { get; set; }
        public IdentityUserNew? AssignedTo { get; set; }
        public DateTime? AssignedDate {get;set;}

    }
}
