using System.ComponentModel.DataAnnotations;

namespace _10433939_PROG6212_POE.Models
{
    public class Lecturer
    {
        [Key]public int LecturerId { get; set; }
        public string LecturerName { get; set; }
        public int HourlyRate { get; set; }
        List<Claim> Claims { get; set; }
    }
}
