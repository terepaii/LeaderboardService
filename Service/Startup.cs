using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

using LeaderboardAPI.Data;
using LeaderboardAPI.Interfaces;
using LeaderboardAPI.Models;
using LeaderboardAPI.Services;


namespace LeaderboardAPI
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
            IConfigurationSection MongoDBSettings = Configuration.GetSection("LeaderboardDatabaseSettings:MongoDB");
            // Configuration 
            services.AddSingleton<LeaderboardMongoDBSettings>();
            services.Configure<LeaderboardMongoDBSettings>(MongoDBSettings);

            // Add Database
            services.AddSingleton<IDatabase, LeaderboardDatabaseMongoDB>();

            // Add the leaderboard service
            services.AddSingleton<ILeaderboardService, LeaderboardService>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());;
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LeaderboardApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LeaderboardApi v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
