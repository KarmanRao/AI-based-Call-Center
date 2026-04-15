using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Data;
using AiCallCenterBackend.Services;
using AiCallCenterBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= DATABASE =================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CallCenterDB"));

// ================= CUSTOM SERVICES =================

builder.Services.AddScoped<EscalationService>();

builder.Services.AddSingleton<SmsQueue>();
builder.Services.AddSingleton<ISmsSender, FakeSmsSender>();
builder.Services.AddHostedService<SmsWorker>();

builder.Services.AddHostedService<EscalationBackgroundService>();

// ================= CORS (🔥 ADD THIS) =================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// ================= SEED DATA =================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.SlaConfigs.Any())
    {
        context.SlaConfigs.AddRange(
            new SlaConfig
            {
                Category = "Electricity",
                InitialTimeHours = 0.0167,
                ReductionHours = 0.0167,
                MinTimeHours = 0.0167
            },
            new SlaConfig
            {
                Category = "Water",
                InitialTimeHours = 0.0167,
                ReductionHours = 0.0167,
                MinTimeHours = 0.0167
            },
            new SlaConfig
            {
                Category = "Road",
                InitialTimeHours = 0.0167,
                ReductionHours = 0.0167,
                MinTimeHours = 0.0167
            }
        );

        context.SaveChanges();
    }
}

// ================= MIDDLEWARE =================

app.UseSwagger();
app.UseSwaggerUI();

// 🔥 IMPORTANT: CORS MUST COME BEFORE MapControllers
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();