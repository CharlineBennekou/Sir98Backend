namespace Sir98Backend.Models.DataTransferObjects
{
    public class InstructorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string? Email { get; set; }
        public string? Number { get; set; }
        public string? Image { get; set; }
    }
}
