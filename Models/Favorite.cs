namespace main.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
