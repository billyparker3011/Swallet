namespace HnMicro.Framework.Data.UnitOfWorks
{
    public interface IUnitOfWorkAsync
    {
        Task<int> SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
