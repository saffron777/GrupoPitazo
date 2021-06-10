using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GP.Core.Models;
using GP.Core.Modules.App.Implementations;
using GP.Core.Modules.App.Interface;
using GP.Core.Repository;
using Microsoft.EntityFrameworkCore;
using GP.Core.Repository.Contracts;
using GP.Core.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using GP.Core.Modules.Account.Interface;
using GP.Core.Modules.Account.Implementations;
using GP.Core.Modules.BO.Interface;
using GP.Core.Modules.BO.Implementations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace GP.Application.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();

            var connection = Configuration.GetConnectionString("AppContext");
            services.AddDbContext<Core.Repository.AppContext>
                (options => options.UseSqlServer(connection));
            services.AddEntityFrameworkSqlServer();
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //context
            services.AddScoped<DbContext, Core.Repository.AppContext>();
            //repositories
            services.AddScoped<IBanqueadasRepository, BanquedasRepository>();
            services.AddScoped<ICarrerasRepository, CarrerasRepository>();
            services.AddScoped<IHipodromosRepository, HipodromosRepository>();
            services.AddScoped<IJugadasRepository, JugadasRepository>();
            services.AddScoped<INotificacionesRepository, NotificacionesRepository>();
            services.AddScoped<ITipoJugadasRepository, TipoJugadasRepository>();
            services.AddScoped<ITokensRepository, TokensRepository>();
            services.AddScoped<ITransaccionesRepository, TransaccionesRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IAceptacionesRepository, AceptacionesRepository>();
            services.AddScoped<IGradeoRepository, GradeoRepository>();
            services.AddScoped<IParametersRepository, ParametersRepository>();
            //services
            services.AddScoped<IBackOfficeServices, BackOfficeServices>();
            services.AddScoped<IAppServices, AppServices>();
            services.AddScoped<IAccountServices, AccountServices>();


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromMinutes(10);
                opts.Cookie.HttpOnly = true;
                opts.Cookie.IsEssential = true;
            });

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod());
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "GPCookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.LogoutPath = "/Home/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddMvc(options => {
            //    options.MaxModelValidationErrors = 50;
            //    //options.AllowValidatingTopLevelNodes = false;
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseCors();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseMvc();
        }
    }
}
