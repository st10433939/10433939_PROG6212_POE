using _10433939_PROG6212_POE.Models;
using System.Security.Claims;

namespace _10433939_PROG6212_POE.Data
{
    public class LecturerData
    {
        private static List<Lecturer> _lecturer = new List<Lecturer>() { };

        private static int _nextId = 4;
        private static int _nextReviewId = 1;

        public static List<Lecturer> GetAllLecturers() => _lecturer.ToList();

        public static Lecturer? GetLecturerById(int id) =>
            _lecturer.FirstOrDefault(b => b.LecturerId == id);

        public static void AddLecturer(Lecturer lecturer)
        {
            lecturer.LecturerId = _nextId;
            _nextId++;
            _lecturer.Add(lecturer);
        }

    }
}
