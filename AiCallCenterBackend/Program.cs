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

// 🔥 CURRENT (InMemory DB for testing)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CallCenterDB"));

// 🔴 FUTURE (Oracle DB)
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// ================= CUSTOM SERVICES =================

builder.Services.AddScoped<EscalationService>();

builder.Services.AddSingleton<SmsQueue>();
builder.Services.AddSingleton<ISmsSender, FakeSmsSender>();
builder.Services.AddHostedService<SmsWorker>();

builder.Services.AddHostedService<EscalationBackgroundService>();

var app = builder.Build();

// ================= SEED DATA (ONLY FOR TESTING) =================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 🔥 Only seed if empty
    if (!context.SlaConfigs.Any())
    {
        context.SlaConfigs.AddRange(
            new SlaConfig
            {
                Category = "Electricity",
                InitialTimeHours = 0.0167,   // ✅ 1 minute
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

app.MapControllers();

app.Run();