using OStats.API;
using OStats.API.Extensions;
using OStats.Infrastructure;
using DataServiceGrpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddJwtBearerAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<Context>();
builder.Services.AddValidators();
builder.Services.AddGrpcClient<DataService.DataServiceClient>( o => {
    o.Address = new Uri("http://dataservice:50051");
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/v1/datasets").WithTags(["Datasets"]).MapDatasetsApi().RequireAuthorization();
app.MapGroup("/v1/projects").WithTags(["Projects"]).MapProjectsApi().RequireAuthorization();
app.MapGroup("/v1/users").WithTags(["Users"]).MapUsersApi().RequireAuthorization();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
