using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Student_mangment.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Student_mangment.Pages
{
    public class ContactModel : PageModel
    {
        private readonly StudentContext _context;

        public ContactModel(StudentContext context)
        {
            _context = context;
            Contact = new Contact(); // Initialize Contact to avoid null reference
        }

        [BindProperty]
        public Contact Contact { get; set; }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; } = null;

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // No need to initialize Contact here as it's already initialized in the constructor
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fix the validation errors.";
                return Page();
            }

        

            try
            {
                _context.Contacts.Add(Contact);
                await _context.SaveChangesAsync();
                SuccessMessage = "Contact posted successfully!";
                Contact = new Contact(); // Reset Contact after successful post
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = $"An error occurred while saving the contact: {ex.Message}";
                return Page();
            }

            return Page();
        }
    }
}