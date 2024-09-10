using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Errors;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.APIs.Extensions;
using StackExchange.Redis;
using Talabat.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // allow depandancy injection for IConnectionMultiplexer
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RidesConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("IdenityConnection"));
            });
            builder.Services.AddCors(Options =>
            {
                Options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });

			builder.Services.AddAplicationServices();

            builder.Services.AddIdentityServices(builder.Configuration);
            

			var app = builder.Build();

            #region Update-datebase
            //StoreContext dbContext = new StoreContext(); //invalid
            //await dbContext.Database.MigrateAsync();
			using var Scope = app.Services.CreateScope();
			// Group Of Services LifeTime Scoped
			var Services = Scope.ServiceProvider;
			// Services Its Self
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
				// Ask CLR For Creating Object From DbContext Explicitly
				var dbContext = Services.GetRequiredService<StoreContext>();
				await dbContext.Database.MigrateAsync();

				// Ask CLR For Creating Object From AppIdentityDbContext Explicitly
				var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbContext.Database.MigrateAsync();

				// Ask CLR For Creating Object From UserManager<AppUser> Explicitly to Seed data
				var UserManagerDbContext = Services.GetRequiredService<UserManager<AppUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(UserManagerDbContext);

				await StoreContextSeed.SeedAsync(dbContext);
			}catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An Error Occured During Appling The Migration"); 
            }

			#endregion

			

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.useSwagggerMiddlewares();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
