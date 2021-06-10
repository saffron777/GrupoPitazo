using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Models;
using GP.Core.Models.ViewModels;

namespace GP.Core.Modules.Account.Interface
{
    public interface IAccountServices
    {
        LoginResponse GetUser(string userId, string password);

        UsersResponse GetUserByLogin(string userName, string password);
    }
}
