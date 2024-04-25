using Microsoft.EntityFrameworkCore;
using PUNDERO.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//SPAJANJE SA VSC
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        builder =>
        {
            builder.WithOrigins("https://localhost:44306/", "https://localhost:3000/", "http://127.0.0.1:5500/")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Register PunderoContext with dependency injection (replace with your actual connection string name)
builder.Services.AddDbContext<PunderoContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//REACT
app.UseCors("AllowReact");

app.Run();

