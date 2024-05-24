using MyProjectDomain.Interfaces;

namespace MyProjectDomain.Entity
{
    public class Report : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedById { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public long? LastUpdatedById { get; set; }
    }
}
