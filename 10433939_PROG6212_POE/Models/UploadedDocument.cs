namespace _10433939_PROG6212_POE.Models
{
    public class UploadedDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsEncrypted { get; set; } = true;
    }
}
