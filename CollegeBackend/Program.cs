using CollegeBackend;
using CollegeBackend.Auth;
using CollegeBackend.Extensions;
using CollegeBackend.Objects.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//Add services to the container.
builder
    .Services
    .Apply(service =>
    {
        service.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationTokenOptions, AuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme,
                _ => { });

        // add controllers
        service.AddControllers();

        // add service, college backend context
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new ArgumentNullException("connectionString",
                                   "Missing connection string value in appsettings.json!");

        service.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        service.AddDbContext<CollegeBackendContext>(options =>
        {
            // options.EnableSensitiveDataLogging();

            options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions
                .UseNetTopologySuite()
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
        service.AddSingleton<IPasswordHasher<User?>, PasswordHasher<User?>>();
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