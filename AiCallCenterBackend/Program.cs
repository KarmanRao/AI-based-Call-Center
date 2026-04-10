using AiCallCenterBackend.Data;
using AiCallCenterBackend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext (In-Memory for now)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CallCenterDB"));

// Register Escalation Service
builder.Services.AddScoped<EscalationService>();

builder.Services.AddSingleton<SmsQueue>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();


// 🔥 BACKGROUND ESCALATION LOOP
using (var scope = app.Services.CreateScope())
{
    var escalationService = scope.ServiceProvider.GetRequiredService<EscalationService>();

    Task.Run(async () =>
    {
        while (true)
        {
            await escalationService.AutoEscalateComplaints();
            await Task.Delay(10000); // every 10 seconds
        }
    });
}

app.Run();