using System;
using System.ComponentModel.DataAnnotations;

namespace Student_mangment.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
    }
}
