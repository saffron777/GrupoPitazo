using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class TipoJugadasRepository : Repository<TipoJugadas>, ITipoJugadasRepository
    {
        public TipoJugadasRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
