namespace HnMicro.Framework.Data.UnitOfWorks
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        void Commit();
        void Rollback();
    }
}
