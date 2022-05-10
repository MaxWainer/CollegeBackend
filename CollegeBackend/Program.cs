using CollegeBackend;
using CollegeBackend.Auth;
using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


//Add services to the container.
builder
    .Services
    .Apply(service =>
    {
        service.AddAuthentication("Basic")
            .AddScheme<AuthenticationTokenOptions, AuthenticationHandler>("Basic", _ =>
            {
            });

        service.AddAuthorization(options =>
        {
            options.AddPolicy("Administrator",
                policy => policy.RequireRole("Administrator"));

            options.AddPolicy("User",
                policy => policy.RequireRole("User"));

            options.AddPolicy("Moderator",
                policy => policy.RequireRole("Moderator"));

            options.AddPolicy("AdministratorAndModerator",
                policy => policy.RequireRole("Administrator", "Moderator"));
        });

        // add controllers
        service.AddControllers();

        // add service, college backend context
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new ArgumentNullException("connectionString",
                                   "Missing connection string value in appsettings.json!");

        service.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        service.AddSingleton<IPasswordHasher<User?>, PasswordHasher<User?>>();
        service.AddDbContext<CollegeBackendContext>(options =>
            options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));
    });

// https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// use https redirects
// app.UseHttpsRedirection();

// enable auth
app.UseAuthentication();
// enable authorize
app.UseAuthorization();

// map controllers
app.MapControllers();

// use status code pages
app.UseStatusCodePages();

// run
app.Run();