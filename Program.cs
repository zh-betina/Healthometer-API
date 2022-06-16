using Healthometer_API.Models;
using Healthometer_API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add servicicon Regular visitses to the container.
builder.Services.Configure<UsersDatabaseSettings>(
    builder.Configuration.GetSection("HealthometerDatabase")
);
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<DocumentsService>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<CategoriesService>();
builder.Services.AddSingleton<DashboardService>();
builder.Services.AddSingleton<MedicalVisitsService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

builder.Host.UseSerilog((context, loggerConf) =>
{
    loggerConf.WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();