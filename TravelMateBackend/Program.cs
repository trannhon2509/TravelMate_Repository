using VNPay_Library;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Thay đổi URL phù hợp
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add VNPAY service to the container.
builder.Services.AddSingleton<IVnpay, Vnpay>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Log ra console
builder.Logging.AddDebug();   // Log ra debug output

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAll"); // Hoặc "AllowAll" tùy nhu cầu

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
