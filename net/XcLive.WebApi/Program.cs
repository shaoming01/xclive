using Microsoft.OpenApi.Models;
using SchemaBuilder.Svc.Helpers;
using SchemaBuilder.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);
// Only add Swagger in development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchemaApi", Version = "1.0.0000" });
    });
}


ApiConfigHelper.Init(builder);

var app = builder.Build();
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" } // 指定默认文档
});
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchemaApi v1");
        c.RoutePrefix = "swagger"; // Swagger UI accessible at /swagger
    });

    var port = 5080; // Ensure this matches your application port
    Console.WriteLine($"Swagger UI available at: http://localhost:{port}/swagger/");
}

app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();