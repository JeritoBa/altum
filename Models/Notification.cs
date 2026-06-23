using System;
using System.ComponentModel.DataAnnotations;
using main.Models.Enums;

namespace main.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
