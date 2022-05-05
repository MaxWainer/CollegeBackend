using CollegeBackend;
using CollegeBackend.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


//Add services to the container.
builder
    .Services
    .Apply(service =>
    {
        // under review
        // -----------------
        // service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        // add new options
        service.AddAuthorization(options =>
        {
            // user policy
            options.AddPolicy("User", policy =>
            {
                // use jwt authentication schemes
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                // we need authenticated user
                policy.RequireAuthenticatedUser();
            });

            // admin policy
            options.AddPolicy("Administrator", policy =>
            {
                // use jwt authentication schemes
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                // we need authenticated user
                policy.RequireAuthenticatedUser();

                // add as requirement, roles auth
                policy.Requirements.Add(new RolesAuthorizationRequirement(new[]
                {
                    "admin"
                }));
            });
        });

        service.AddAuthentication(options =>
        { 
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
            options.Audience = builder.Configuration["Auth0:Audience"];
        });

        // add controllers
        service.AddControllers();

        // add service, college backend context
        service
            .AddDiService(new CollegeBackendContext(new DbContextOptions<CollegeBackendContext>()));
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
app.UseHttpsRedirection();

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