using _10433939_PROG6212_POE.Models;
using System.ComponentModel.DataAnnotations;

namespace _10433939_PROG6212_POE.Models
{
    public class Claim
    {
        [Key]public int Id { get; set; }
        public string LecturerName { get; set; }
        public int HoursWorked { get; set; }
        public int HourlyRate { get; set; }
        public int Balance { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string SubmittedBy { get; set; }
        public ClaimStatus Status { get; set; }
        public string ReviewedBy { get; set; }
        public string? Comments { get; set; }
        public DateTime ReviewedDate { get; set; }
        public Lecturer? lecturers { get; set; }
        public List<UploadedDocument> Documents { get; set; }
        public List<ClaimReview> Reviews { get; set; } = new List<ClaimReview>();
    }
}
