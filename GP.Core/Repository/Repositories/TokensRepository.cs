using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class TokensRepository : Repository<Tokens>, ITokensRepository
    {
        public TokensRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
