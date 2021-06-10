using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class AceptacionesRepository : Repository<Aceptaciones>, IAceptacionesRepository
    {
        public AceptacionesRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
