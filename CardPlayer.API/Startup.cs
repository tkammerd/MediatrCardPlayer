using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardPlayer.API
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
            //services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllers(opt =>  // or AddMvc()
            {
                // remove formatter that turns nulls into 204 - No Content responses
                // this formatter breaks Angular's Http response JSON parsing
                // from https://weblog.west-wind.com/posts/2020/Feb/24/Null-API-Responses-and-HTTP-204-Results-in-ASPNET-Core
                opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            });
            services.AddMediatR(typeof(Startup));
            services.AddDbContext<GameContext>(options => options.UseInMemoryDatabase(databaseName: "GameDB"));
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
                endpoints.MapControllers();  // Specifies attribute routing
            });

            //using (var serviceScope = app.ApplicationServices.CreateScope())
            //{
            //    var CardContext = serviceScope.ServiceProvider.GetService<CardContext>();
            //    CardContext.Database.EnsureCreated();
            //    var GameContext = serviceScope.ServiceProvider.GetService<GameContext>();
            //    GameContext.Database.EnsureCreated();
            //}
        }
    }
}
