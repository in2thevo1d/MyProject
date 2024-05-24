namespace MyProjectDomain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public long CreatedById { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public long? LastUpdatedById { get; set; }
    }
}
