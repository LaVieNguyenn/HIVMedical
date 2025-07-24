using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add controllers
builder.Services.AddControllers();

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Add SwaggerForOcelot
builder.Services.AddSwaggerForOcelot(builder.Configuration);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
        opt.ReConfigureUpstreamSwaggerJson = AlterUpstream;
    });
}

// Only use HTTPS redirection if not in Docker or if HTTPS port is configured
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")))
{
    app.UseHttpsRedirection();
}
app.UseCors("CorsPolicy");

// Map controllers
app.MapControllers();

// Ocelot middleware
await app.UseOcelot();

app.Run();

string AlterUpstream(HttpContext context, string swaggerJson)
{
    return swaggerJson.Replace("localhost", "hivmedical.com");
}
