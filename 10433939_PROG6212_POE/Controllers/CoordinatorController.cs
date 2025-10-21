using _10433939_PROG6212_POE.Models;
using _10433939_PROG6212_POE.Data;
using _10433939_PROG6212_POE.Models;
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
                    _ => claims
                };

                ViewBag.PendingCount = ClaimData.GetPendingCount();
                ViewBag.ApprovedCount = ClaimData.GetApprovedCount();
                ViewBag.DeclinedCount = ClaimData.GetDeclinedCount();

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

        // POST: /Manager/Decline - CREATES REVIEW RECORD
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

                string reviewedBy = "Admin User";
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
