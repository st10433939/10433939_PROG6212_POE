using _10433939_PROG6212_POE.Models;
using _10433939_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;

namespace _10433939_PROG6212_POE.Controllers
{
    public class CoordinatorController : Controller
    {
        public IActionResult Index(string filter = "all")
        {
            try
            {
                var claims = ClaimData.GetAllClaims();
                ViewBag.Filter = filter;

                claims = filter.ToLower()
                    switch
                {
                    "pending" => ClaimData.GetClaimsByStatus(ClaimStatus.Pending),
                    "approved" => ClaimData.GetClaimsByStatus(ClaimStatus.Approved),
                    "declined" => ClaimData.GetClaimsByStatus(ClaimStatus.Declined),
                    "verified" => ClaimData.GetClaimsByStatus(ClaimStatus.Verified),
                    _ => claims
                };

                ViewBag.PendingCount = ClaimData.GetPendingCount();
                ViewBag.ApprovedCount = ClaimData.GetApprovedCount();
                ViewBag.DeclinedCount = ClaimData.GetDeclinedCount();
                ViewBag.VerifiedCount = ClaimData.GetVerifiedCount();

                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load Claims.";
                return View(new List<Claim>());
            }

            return View();
        }
        public IActionResult Review(int id)
        {
            try
            {
                var claim = ClaimData.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(claim);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading Claim.";
                return RedirectToAction(nameof(Index));
            }
        }
        // POST: /Coordinator/Verified - CREATES REVIEW RECORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Verify(int id, string? comments)
        {
            try
            {
                string reviewedBy = "Program Coordinator";
                string reviewComments = string.IsNullOrWhiteSpace(comments)
                    ? "Approved for claim"
                    : comments;

                var success = ClaimData.UpdateClaimStatus(id, ClaimStatus.Verified, reviewedBy, reviewComments);

                if (success)
                {
                    TempData["Success"] = "Claim verified successfully!";
                }
                else
                {
                    TempData["Error"] = "Claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error verifying claim.";
                return RedirectToAction(nameof(Index));
            }
        }
        // POST: /Coordinator/Decline - CREATES REVIEW RECORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decline(int id, string? comments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comments))
                {
                    TempData["Error"] = "Please provide a reason for declining.";
                    return RedirectToAction(nameof(Review), new { id });
                }

                string reviewedBy = "Program Coordinator";
                var success = ClaimData.UpdateClaimStatus(id, ClaimStatus.Declined, reviewedBy, comments);

                if (success)
                {
                    TempData["Success"] = "Claim declined.";
                }
                else
                {
                    TempData["Error"] = "Claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error declining Claim.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
