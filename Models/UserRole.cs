namespace TaskManagementSystem.Models
{
    public class UserRole
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }    
        public bool IsAssigned { get; set; }  
    }
}
