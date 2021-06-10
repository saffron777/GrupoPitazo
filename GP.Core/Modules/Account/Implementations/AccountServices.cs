using System;
using System.Collections.Generic;
using System.Text;
using GP.Core.Logging;
using GP.Core.Models;
using GP.Core.Utilities;
using GP.Core.Entities;
using GP.Core.Modules.BO.Interface;
using GP.Core.Repository.Contracts;
using GP.Core.Models.ViewModels;
using GP.Core.Modules.Account.Interface;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace GP.Core.Modules.Account.Implementations
{
    public class AccountServices : IAccountServices
    {
        private readonly ITransaccionesRepository _transaccionesRepository;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _usersRepository;
        public AccountServices(ITransaccionesRepository transaccionesRepository, IConfiguration configuration, IUsersRepository usersRepository)
        {
            _transaccionesRepository = transaccionesRepository;
            _configuration = configuration;
            _usersRepository = usersRepository;

            Utils.settings = new AppSettings
            {
                usuario = _configuration.GetSection("appSettings").GetSection("usuario").Value,
                clave = _configuration.GetSection("appSettings").GetSection("clave").Value,
                clavecliente = _configuration.GetSection("appSettings").GetSection("clavecliente").Value,
                password = _configuration.GetSection("appSettings").GetSection("password").Value,
                SecurityKey = _configuration.GetSection("appSettings").GetSection("SecurityKey").Value,
                urlLoginGCIT = _configuration.GetSection("appSettings").GetSection("urlLoginGCIT").Value,
                urlWSA = _configuration.GetSection("appSettings").GetSection("urlWSA").Value,
            };
        }
        public LoginResponse GetUser(string userId, string password)
        {
            LoginResponse login = null;

            try
            {
                login = Utils.GetLogin(userId);

                if (login != null && login.Existe == "1")
                {

                    if (login.ClaveDeCliente == password)
                        return login;
                    else
                        return null;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }


            return login;
        }

        public UsersResponse GetUserByLogin(string userName, string password)
        {
            UsersResponse login = null;

            try
            {
                Expression<Func<Users, bool>> expression = exp => exp.UserName.ToUpper() == userName.ToUpper();

                login = _usersRepository.Get(expression).Select(s => new UsersResponse
                {
                    UserID = s.UserID,
                    UserName = s.UserName,
                    Password = s.Password,
                    Nombre = s.Nombre,
                    Apellido = s.Apellido,
                    TokenID = s.TokenID,
                    FechaCreacion = s.FechaCreacion,
                    Activo = s.Activo
                }).SingleOrDefault();

                if (login != null && login.Activo)
                {

                    if (login.Password == password)
                        return login;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }


            return login;
        }
    }
}
