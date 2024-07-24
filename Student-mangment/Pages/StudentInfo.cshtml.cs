using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Student_mangment.Models;
using System.ComponentModel.DataAnnotations;

namespace Student_mangment.Pages
{
    public class StudentInfoModel : PageModel
    {
        private readonly StudentContext _context;

        public StudentInfoModel(StudentContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Student ID is required.")]
        [StringLength(10, ErrorMessage = "Student ID cannot be longer than 10 characters.")]
        public string StudentID { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Student Name is required.")]
        [StringLength(50, ErrorMessage = "Student Name cannot be longer than 50 characters.")]
        public string StudentName { get; set; }

        [BindProperty]
        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public string Address { get; set; }

        [BindProperty]
        [Range(1, 100, ErrorMessage = "Age must be between 1 and 100.")]
        public string Age { get; set; }

        [BindProperty]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNo { get; set; }

        [BindProperty]
        public string Gender { get; set; } // Consider using an Enum for predefined values

        [BindProperty]
        public List<string> Skills { get; set; } = new List<string>();


        public List<Student> Students { get; set; } = new List<Student>();

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Students = await _context.Students.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fix the validation errors.";
                Students = await _context.Students.ToListAsync();
                return Page();
            }

            if (await _context.Students.AnyAsync(s => s.StudentID == StudentID))
            {
                ErrorMessage = "Student ID already exists.";
                Students = await _context.Students.ToListAsync();
                return Page();
            }

            var newStudent = new Student
            {
                StudentID = StudentID,
                StudentName = StudentName,
                Address = Address,
                Age = Age,
                PhoneNo = PhoneNo,
                Gender = Gender,
                Skills = string.Join(", ", Skills)
            };

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == StudentID);
            if (student == null)
            {
                ErrorMessage = "Student not found.";
                Students = await _context.Students.ToListAsync();
                return Page();
            }

            student.StudentName = StudentName;
            student.Address = Address;
            student.Age = Age;
            student.PhoneNo = PhoneNo;
            student.Gender = Gender;
            student.Skills = string.Join(", ", Skills);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentID == StudentID);
            if (student == null)
            {
                ErrorMessage = "Student not found.";
                Students = await _context.Students.ToListAsync();
                return Page();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSelectAsync()
        {
            Students = await _context.Students.Where(s => s.StudentID == StudentID).ToListAsync();
            if (!Students.Any())
            {
                ErrorMessage = "Student not found.";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            var students = await _context.Students.ToListAsync();

            var filePath = Path.Combine(Path.GetTempPath(), "Students.xlsx");

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Students" };
                sheets.Append(sheet);

                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                Row headerRow = new Row();
                headerRow.Append(
                    new Cell() { CellValue = new CellValue("Student ID"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Student Name"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Address"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Age"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Phone No"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Gender"), DataType = CellValues.String },
                    new Cell() { CellValue = new CellValue("Skills"), DataType = CellValues.String }
                );
                sheetData.AppendChild(headerRow);

                foreach (var student in students)
                {
                    Row dataRow = new Row();
                    dataRow.Append(
                        new Cell() { CellValue = new CellValue(student.StudentID), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.StudentName), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.Address), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.Age), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.PhoneNo), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.Gender), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(student.Skills), DataType = CellValues.String }
                    );
                    sheetData.AppendChild(dataRow);
                }

                workbookPart.Workbook.Save();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
        }
    }
}
