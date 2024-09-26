
using Microsoft.EntityFrameworkCore;
using LAB1API.Model;

namespace LAB1API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
           );
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(cors =>
            {
                cors.AddPolicy("FreeTrial", p =>
                {
                    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("FreeTrial");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
