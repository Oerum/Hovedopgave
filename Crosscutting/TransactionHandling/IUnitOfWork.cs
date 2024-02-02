using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Crosscutting.TransactionHandling;

public interface IUnitOfWork<T> where T : DbContext
{
    Task CreateTransaction(IsolationLevel level);
    Task Commit();
    Task Rollback();
    Task<IDbContextTransaction> CheckCurrentTransaction();
}