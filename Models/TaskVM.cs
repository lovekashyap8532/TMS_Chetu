namespace TaskManagementSystem.Models
{
    public class TaskVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public IdentityUserNew? AssignedTo { get; set; }
        public DateTime? AssignedDate { get; set; }

    }
}
