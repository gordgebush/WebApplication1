using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connection string from configuration (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add PostgreSQL database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        builder => builder.WithOrigins("http://localhost:56384") // Your React app URL
            .AllowAnyMethod()
            .AllowAnyHeader());
});



// Add controllers
builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Add the FileUploadOperationFilter to handle file upload in Swagger UI
    c.OperationFilter<FileUploadOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowLocalhost");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters)
        {
            // Check if the parameter is an IFormFile, and if so, mark it as a file type in Swagger
            if (parameter.Schema?.Type == "string" && parameter.Schema?.Format == "binary")
            {
                parameter.Schema.Type = "file";
            }
        }
    }
}