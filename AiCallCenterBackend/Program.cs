using AiCallCenterBackend.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();          // ✅ enables Controllers (like ComplaintsController)
builder.Services.AddEndpointsApiExplorer(); // ✅ needed for Swagger
builder.Services.AddSwaggerGen();           // ✅ Swagger UI
builder.Services.AddHostedService<EscalationService>();

builder.Services.AddSingleton<SmsQueue>();
builder.Services.AddSingleton<ISmsSender, FakeSmsSender>();
builder.Services.AddHostedService<SmsWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // ✅ maps all controller routes (api/Complaints, etc.)

app.Run();
