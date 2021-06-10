using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class TransaccionesRepository : Repository<Transacciones>, ITransaccionesRepository
    {
        public TransaccionesRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
