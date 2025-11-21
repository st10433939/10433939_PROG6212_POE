using _10433939_PROG6212_POE.Models;
using _10433939_PROG6212_POE.Data;
using Microsoft.AspNetCore.Mvc;

namespace _10433939_PROG6212_POE.Controllers
{
    public class HRController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                var lecturers = LecturerData.GetAllLecturers();
                return View(lecturers);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Unable to load Lecturers.";
                return View(new List<Lecturer>());
            }
        }

        public IActionResult AddLecturer()
        {
            return View();
        }

        //Post /HR/AddLecturer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLecturer(Lecturer lecturer)
        {
            try
            {
                if (string.IsNullOrEmpty(lecturer.LecturerName))
                {
                    ViewBag.Error = "Lecturer is required.";
                    return View(lecturer);
                }
                if (string.IsNullOrEmpty(lecturer.HourlyRate.ToString()))
                {
                    ViewBag.Error = "Lecturer rate required.";
                    return View(lecturer);
                }

                LecturerData.AddLecturer(lecturer);
                TempData["Success"] = "Lecturer added sucessfully";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ViewBag["Error"] = "Error handling lecturer" + ex.Message;
                return View(lecturer);
            }
        }
    }
}
