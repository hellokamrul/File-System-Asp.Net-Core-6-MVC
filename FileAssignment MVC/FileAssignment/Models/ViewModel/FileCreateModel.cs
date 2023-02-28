namespace FileAssignment.Models.ViewModel
{
    public class FileCreateModel
    {
        public int Id { get; set; }

        public int? Uid { get; set; }

        public string? FileName { get; set; }

        public string? ContentType { get; set; }

        public IFormFile FilePath { get; set; }

        public long? Length { get; set; }

        public virtual User? UidNavigation { get; set; }
    }
}
