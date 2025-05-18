using Authentication.Application.Interfaces;
using Authentication.Infrastructure.Data;
using Authentication.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;
        public IUserRepository Users { get; }

        public UnitOfWork(AuthDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
