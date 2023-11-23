using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaTicket.DataAccess;
using CinemaTicket.DataAccess.Interfaces;
using CinemaTicket.BusinessLogicServices;
using CinemaTicket.BusinessLogic.Interfaces;

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
