using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.DS.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Allinone.DLL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<DSItem> DSItem { get; }
        IRepository<DSItemSub> DSItemSub { get; }
        IRepository<DSTransaction> DSTransaction { get; }

        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DSContext _context;
        private IDbContextTransaction _transaction;

        public IRepository<DSItem> DSItem { get; }
        public IRepository<DSItemSub> DSItemSub { get; }
        public IRepository<DSTransaction> DSTransaction { get; }

        public UnitOfWork(DSContext context)
        {
            _context = context;
            DSItem = new Repository<DSItem>(_context);
            DSItemSub = new Repository<DSItemSub>(_context);
            DSTransaction = new Repository<DSTransaction>(_context);
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction?.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }

    //public class UnitOfWork : IUnitOfWork
    //{
    //    private readonly DSContext _context;
    //    private IDbContextTransaction? _transaction;

    //    public UnitOfWork(DSContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task BeginTransactionAsync()
    //    {
    //        _transaction = await _context.Database.BeginTransactionAsync();
    //    }

    //    public async Task CommitAsync()
    //    {
    //        if (_transaction != null)
    //        {
    //            await _transaction.CommitAsync();
    //            await _transaction.DisposeAsync();
    //        }
    //    }

    //    public async Task RollbackAsync()
    //    {
    //        if (_transaction != null)
    //        {
    //            await _transaction.RollbackAsync();
    //            await _transaction.DisposeAsync();
    //        }
    //    }
    //}
}
