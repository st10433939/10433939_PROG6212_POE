using _10433939_PROG6212_POE.Models;
using AspNetCoreGeneratedDocument;

namespace _10433939_PROG6212_POE.Data
{
    public class ClaimData
    {
        private static List<Claim> _claims = new List<Claim>()
{
    new Claim
    {
        Id = 1,
        HoursWorked = 20,
        HourlyRate = 200,
        AdditionalNotes = "None.",
        SubmittedDate = DateTime.Now.AddDays(-5),
        Status = ClaimStatus.Pending,
        Documents = new List<UploadedDocument>()
    }
};

        private static int _nextId = 4;
        private static int _nextReviewId = 1;

        public static List<Claim> GetAllClaims() => _claims.ToList();

        public static Claim? GetClaimById(int id) =>
            _claims.FirstOrDefault(b => b.Id == id);

        public static List<Claim> GetClaimsByStatus(ClaimStatus status) =>
            _claims.Where(b => b.Status == status).ToList();

        public static void AddClaim(Claim claim)
        {
            claim.Id = _nextId;
            _nextId++;
            claim.SubmittedDate = DateTime.Now;
            claim.Status = ClaimStatus.Pending;
            _claims.Add(claim);
        }

        public static bool UpdateClaimStatus(int id, ClaimStatus newStatus, string reviewedBy, string comments)
        {
            var claim = GetClaimById(id);
            if (claim == null) return false;

            // CREATE REVIEW RECORD
            var review = new ClaimReview
            {

                Id = _nextReviewId,
                ClaimId = id,
                ReviewerName = reviewedBy,
                ReviewerRole = "Administrator",
                ReviewDate = DateTime.Now,
                Decision = newStatus,
                Comments = comments
            };
            _nextReviewId++;

            claim.Reviews.Add(review);

            // UPDATE Claim STATUS
            claim.Status = newStatus;
            claim.ReviewedBy = reviewedBy;
            claim.ReviewedDate = DateTime.Now;

            return true;
        }

        public static int GetPendingCount() =>
            _claims.Count(b => b.Status == ClaimStatus.Pending);

        public static int GetApprovedCount() =>
            _claims.Count(b => b.Status == ClaimStatus.Approved);

        public static int GetVerifiedCount() =>
            _claims.Count(b => b.Status == ClaimStatus.Verified);

        public static int GetDeclinedCount() =>
            _claims.Count(b => b.Status == ClaimStatus.Declined);
    }
}


