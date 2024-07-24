using System.ComponentModel.DataAnnotations;

namespace Student_mangment.Models
{
    public class Student
    {
        [Key]
        [Required(ErrorMessage = "Student ID is required.")]
        [StringLength(10, ErrorMessage = "Student ID cannot be longer than 10 characters.")]
        public string StudentID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student Name is required.")]
        [StringLength(50, ErrorMessage = "Student Name cannot be longer than 50 characters.")]
        public string StudentName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public string Address { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100.")]
        public string Age { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNo { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Skills { get; set; } = string.Empty;
    }
}
