using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serilog;

namespace CbrRatesTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) => configuration.
                    ReadFrom.Configuration(context.Configuration).
                    ReadFrom.Services(services));

                builder.Services.AddControllers();
                builder.Services.AddOpenApi();
                builder.Services.AddSwaggerGen();
                builder.Services.AddDbContext<Data.AppDbContext>(options =>
                    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

                var app = builder.Build();

                app.UseSerilogRequestLogging();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
