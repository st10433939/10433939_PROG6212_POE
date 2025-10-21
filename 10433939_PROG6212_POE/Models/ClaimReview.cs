using _10433939_PROG6212_POE.Models;

namespace _10433939_PROG6212_POE.Models
{
    public class ClaimReview
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string ReviewerRole { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public ClaimStatus Decision { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}
