using Microsoft.EntityFrameworkCore;
using AiCallCenterBackend.Data;
using AiCallCenterBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// ================= CONTROLLERS + SWAGGER =================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================= DATABASE =================

// 🔥 CURRENT (for development)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CallCenterDB"));

// 🟡 FUTURE (Oracle)
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// ================= SERVICES =================

// Escalation logic
builder.Services.AddScoped<EscalationService>();

// 🔥 Background escalation (CORRECT WAY)
builder.Services.AddHostedService<EscalationBackgroundService>();

// SMS system
builder.Services.AddSingleton<SmsQueue>();
builder.Services.AddSingleton<ISmsSender, FakeSmsSender>();
builder.Services.AddHostedService<SmsWorker>();

// ================= BUILD APP =================
var app = builder.Build();

// ================= MIDDLEWARE =================
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// ================= RUN =================
app.Run();