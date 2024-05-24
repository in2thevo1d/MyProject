using MyProjectDomain.Interfaces;

namespace MyProjectDomain.Entity
{
    public class User : IEntityId<long>, IAuditable
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Report> Reports { get; set; }
        public List<Role> Roles { get; set; }
        public UserToken UserToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedById { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public long? LastUpdatedById { get; set; }
    }
}
