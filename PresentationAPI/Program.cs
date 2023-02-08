using AuthServer.Core.Configuration;
using SharedLibrary.Configurations;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOption"));

//code below called "Options Pattern" Converts appsettings section to class object
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
