using OStats.API;
using OStats.API.Extensions;
using OStats.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddJwtBearerAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<Context>();
builder.Services.AddValidators();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/v1/users").WithTags(["Users"]).MapUsersApi().RequireAuthorization();
app.MapGroup("/v1/projects").WithTags(["Projects"]).MapProjectsApi().RequireAuthorization();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
