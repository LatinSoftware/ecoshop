using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService;
using UserService.Database;
using UserService.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices(builder.Configuration);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(UConstants.PolicyName, o => o.RequireRole(UConstants.AdminRole));

builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddApiVersioning().AddApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("test", () =>
{
    return Results.Ok("respuesta");
}).RequireAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await DatabaseSeed.SeedAsync(context);
}

app.UseHttpsRedirection();


ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.UseAuthentication();
app.UseAuthorization();

app.Run();


