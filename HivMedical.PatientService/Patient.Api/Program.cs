using Microsoft.OpenApi.Models;
using Patient.Application.Services;
using Patient.Application.Services.Interfaces;
using Patient.Application.BackgroundServices;
using Patient.Infrastructure.DependencyInjection;
using SharedLibrary.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Add JWT Authentication from SharedLibrary
builder.Services.AddLibrary(builder.Configuration);

// Add Messaging (RabbitMQ EventBus)
builder.Services.AddMessaging(builder.Configuration);

// Add Application Services
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<MedicalRecordService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<MedicationService>();
builder.Services.AddScoped<PatientMedicationService>();
// Add Notification Services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<INotificationTemplateService, NotificationTemplateService>();
builder.Services.AddSingleton<INotificationLogService, NotificationLogService>();

// Add Event Handlers
builder.Services.AddScoped<UserRegisteredEventHandler>();
builder.Services.AddScoped<PatientCreatedEventHandler>();
builder.Services.AddScoped<PatientUpdatedEventHandler>();
builder.Services.AddScoped<AppointmentCreatedEventHandler>();
builder.Services.AddScoped<MedicationPrescribedEventHandler>();
builder.Services.AddScoped<MedicationRefillDueEventHandler>();

// Add Background Services
builder.Services.AddHostedService<EmailNotificationBackgroundService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Patient API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
