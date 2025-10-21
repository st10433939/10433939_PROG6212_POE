using _10433939_PROG6212_POE.Models;
using _10433939_PROG6212_POE.Services;
using _10433939_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace _10433939_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        public readonly IWebHostEnvironment _environment;
        public readonly FileEncryptionService _encryptionService;

        public ClaimsController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _encryptionService = new FileEncryptionService();
        }
        public IActionResult Index()
        {
            try
            {
                var claims = ClaimData.GetAllClaims();
                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load claims.";
                return View(new List<Claim>());
            }

        }

        public IActionResult Add()
        {
            return View();
        }

        //Post /Claims/Add - Add form data to the datastore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Claim claim, List<IFormFile> documents)
        {
            try
            {
                if (string.IsNullOrEmpty(claim.LecturerName))
                {
                    ViewBag.Error = "Claim lecturer is required.";
                    return View(claim);
                }
                if (string.IsNullOrEmpty(claim.HoursWorked.ToString()))
                {
                    ViewBag.Error = "Claim hours required.";
                    return View(claim);
                }
                if (string.IsNullOrEmpty(claim.HourlyRate.ToString()))
                {
                    ViewBag.Error = "Claim rate required.";
                    return View(claim);
                }
                if (documents != null && documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        if (file.Length > 0)
                        {
                            var allowedExtensions = new[] { ".pdf", ".docx", ".txt", ".xlsx" };
                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(extension))
                            {
                                ViewBag.Error = $"File extension {extension} not allowed.";
                                return View(claim);
                            }

                            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + ".encrypted";
                            var encryptedFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = file.OpenReadStream())
                            {
                                await _encryptionService.EncryptFileAsync(fileStream, encryptedFilePath);
                            }

                            claim.Documents.Add(new UploadedDocument
                            {
                                FileName = file.FileName,
                                FilePath = "/uploads/" + uniqueFileName,
                                FileSize = file.Length,
                                IsEncrypted = true
                            });
                        }
                    }
                }

                ClaimData.AddClaim(claim);
                TempData["Success"] = "Claim added sucessfully";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag["Error"] = "Error handling claim" + ex.Message;
                return View(claim);
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var claim = ClaimData.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return View();
                }
                return View(claim);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading claim.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DownloadDocument(int claimId, int docId)
        {
            try
            {
                var claim = ClaimData.GetClaimById(claimId);
                if (claim == null) { return NotFound("Claim not found."); }

                var document = claim.Documents.FirstOrDefault(doc => doc.Id == docId);
                if (document == null) { return NotFound("Document not found."); }

                var encryptedFilePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (!System.IO.File.Exists(encryptedFilePath)) return NotFound("File not found;");

                var decryptedStream = await _encryptionService.DecryptFileAsync(encryptedFilePath);

                var contentType = Path.GetExtension(document.FileName).ToLower()
                    switch
                {
                    ".pdf" => "application/pdf",
                    ".txt" => "application/txt",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                return File(decryptedStream, contentType, document.FileName);

            }
            catch (Exception ex)
            {
                return BadRequest("Error downloading file: " + ex.Message);
            }
        }

        // POST: /Admin/Approve - CREATES REVIEW RECORD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id, string? comments)
        {
            try
            {
                string reviewedBy = "Admin User";
                string reviewComments = string.IsNullOrWhiteSpace(comments)
                    ? "Approved for library collection"
                    : comments;

                var success = ClaimData.UpdateClaimStatus(id, ClaimStatus.Approved, reviewedBy, reviewComments);

                if (success)
                {
                    TempData["Success"] = "Claim approved successfully!";
                }
                else
                {
                    TempData["Error"] = "Claim not found.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error approving claim.";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}