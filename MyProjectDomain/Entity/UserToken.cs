using MyProjectDomain.Interfaces;

namespace MyProjectDomain.Entity
{
    public class UserToken : IEntityId<long>
    {
        public long Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpityTime { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
    }
}
