using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.DataAccess;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.BusinessLogicServices;
using CinemaTicket.BusinessLogic.Interfaces;
using CinemaTicket.Infrastructure.Settings;

namespace CinemaTicket.WebApi
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
            services.AddControllers();
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<CinemaManagerContext>(options => options.UseSqlServer(connection));
            services.AddScoped<IGenreDataAccess, GenreDataAccess>();
            services.AddScoped<IMovieDataAccess, MovieDataAccess>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IHallDataAccess, HallDataAccess>();
            services.AddScoped<IHallService, HallService>();
            services.AddScoped<IRowDataAccess, RowDataAccess>();
            services.AddScoped<IRowService, RowService>();
            services.AddScoped<IPlaceDataAccess, PlaceDataAccess>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<ISessionDataAccess, SessionDataAccess>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ITicketDataAccess, TicketDataAccess>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IAccountDataAccess, AccountDataAccess>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBaseDataAccess, BaseDataAccess>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IReportService, ReportService>();

            services.Configure<FileServiceSettings>(Configuration.GetSection("FileServiceSettings"));

            services.AddSwaggerGen();
            services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (!env.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CinemaTicket API V1");
                        c.RoutePrefix = string.Empty;
                    });
            }
            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
