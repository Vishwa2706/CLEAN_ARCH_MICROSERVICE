namespace User.Domain.Models
{
    public class RefTerm
    {
        public int Id { get; set; }

        public string TermType { get; set; } = null!;

        public string TermValue { get; set; } = null!;
        
        public bool IsActive { get; set; } = true;
    }
}
