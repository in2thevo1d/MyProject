namespace MyProjectDomain.Interfaces.Repositories
{
    public interface IStateSaveChanges
    {
        Task<int> SaveChangesAsync();
    }
}
