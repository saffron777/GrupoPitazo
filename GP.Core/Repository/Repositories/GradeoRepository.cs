using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class GradeoRepository : Repository<Gradeo>, IGradeoRepository
    {
        public GradeoRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
