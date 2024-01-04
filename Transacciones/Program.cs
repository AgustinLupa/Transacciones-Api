
using Microsoft.EntityFrameworkCore;
using Transacciones.Datos.Modelos;
using Transacciones.Interface;
using Transacciones.Repositorio;

namespace Transacciones
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var cadena = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ModelContext>(
                x => x.UseOracle(cadena, options => options.UseOracleSQLCompatibility("11"))
                );

            builder.Services.AddScoped<IHistorialsaldoRepositorio, HistorialsaldoRepositorio>();
            builder.Services.AddScoped<ICarteraRepositorio, CarteraRepositorio>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
    }
}
