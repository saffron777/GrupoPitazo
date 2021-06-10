using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class HipodromosRepository : Repository<Hipodromos>, IHipodromosRepository
    {
        public HipodromosRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
