﻿using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Repository;
using GP.Core.Entities;
using GP.Core.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GP.Core.Repository.Repositories
{
    public class ParametersRepository : Repository<Parameters>, IParametersRepository
    {
        public ParametersRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
