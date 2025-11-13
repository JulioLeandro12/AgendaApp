// using Microsoft.EntityFrameworkCore;
// using ContactApi.Data;
// using System.Text.Json.Serialization;

// var builder = WebApplication.CreateBuilder(args);

// // Configuration for PostgreSQL
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                      ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
//                      ?? "Host=postgres;Port=5432;Database=contactsdb;Username=postgres;Password=postgres";

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(connectionString)
// );

// builder.Services.AddControllers()
//     .AddJsonOptions(opts => {
//         opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//     });

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// // MVC views are not required for API but kept if needed
// builder.Services.AddControllersWithViews();

// var app = builder.Build();

// // Apply migrations & seed database on startup (inside container, will run automatically)
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
//     SeedData.Initialize(db);
// }

// if (app.Environment.IsDevelopment())
// {
//     app.UseDeveloperExceptionPage();
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseRouting();
// app.UseAuthorization();

// app.MapControllers();

// app.Run();


// --------------------------------------------------------------

// using Microsoft.EntityFrameworkCore;
// using ContactApi.Data;
// using System.Text.Json.Serialization;
// using Microsoft.AspNetCore.Diagnostics;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;

// using Microsoft.EntityFrameworkCore;
// using ContactApi.Data;
// using ContactApi.Repositories;
// using ContactApi.Services;
// using FluentValidation.AspNetCore;
// using System.Text.Json.Serialization;
// using AutoMapper;
// using Microsoft.OpenApi.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Configuration for PostgreSQL
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                      ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
//                      ?? "Host=postgres;Port=5432;Database=contactsdb;Username=postgres;Password=postgres";

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(connectionString)
// );

// builder.Services.AddControllers()
//     .AddJsonOptions(opts =>
//     {
//         opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//     });
//     .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<ContactApi.Validators.ContactValidator>());

// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.AddScoped<IContactRepository, ContactRepository>();
// builder.Services.AddScoped<IContactService, ContactService>();

// builder.Services.AddEndpointsApiExplorer();
// // builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(c => {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Agenda", Version = "v1" });
// });
// // builder.Services.AddControllersWithViews(); // MVC views are not required for API but kept if needed

// var app = builder.Build();

// // Apply migrations & seed database on startup (inside container, will run automatically)
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
//     SeedData.Initialize(db);
// }

// // Development: developer page + swagger
// if (app.Environment.IsDevelopment())
// {
//     app.UseDeveloperExceptionPage();
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// else
// {
//     // in production, use the exception handler global (hypothetical scenario)
//     app.UseExceptionHandler("/error");
// }

// // global exception endpoint for production
// app.Map("/error", (HttpContext http) =>
// {
//     // capture exception
//     var feature = http.Features.Get<IExceptionHandlerFeature>();
//     var exception = feature?.Error;

//     // log the exception
//     var logger = http.RequestServices.GetRequiredService<ILogger<Program>>();
//     if (exception != null)
//     {
//         logger.LogError(exception, "Unhandled exception captured by UseExceptionHandler");
//     }

//     // in developer environments
//     var detail = app.Environment.IsDevelopment() ? exception?.ToString() : "Um erro ocorreu no servidor.";

//     return Results.Problem(
//         title: "Erro interno no servidor",
//         detail: detail,
//         statusCode: 500
//     );
// });

// app.UseHttpsRedirection();

// app.UseRouting();

// app.UseAuthorization();

// app.MapControllers();

// app.Run();

// --------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using ContactApi.Data;
using ContactApi.Repositories;
using ContactApi.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL connection configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
                     ?? "Host=postgres;Port=5432;Database=contactsdb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// configuration of Controllers and FluentValidation
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ContactApi.Validators.ContactDtoValidator>();

// Customize API behavior to suppress automatic model state validation (only use the FluentValidation)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// dependency Injection (Repository + Service)
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Agenda API",
        Version = "v1",
        Description = "API de agenda para gerenciamento de contatos"
    });
});

// // Customize API behavior to suppress automatic model state validation (only use the FluentValidation)
// builder.Services.Configure<ApiBehaviorOptions>(options =>
// {
//     options.SuppressModelStateInvalidFilter = true;
// });


var app = builder.Build();

// Apply migrations & seed database on startup (inside container, will run automatically)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    SeedData.Initialize(db);
}

// environment: Development vs Production
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

// global exception endpoint for production (hypothetical scenario)
app.Map("/error", (HttpContext http) =>
{
    var feature = http.Features.Get<IExceptionHandlerFeature>();
    var exception = feature?.Error;

    var logger = http.RequestServices.GetRequiredService<ILogger<Program>>();
    if (exception != null)
        logger.LogError(exception, "Unhandled exception captured by UseExceptionHandler");

    var detail = app.Environment.IsDevelopment()
        ? exception?.ToString()
        : "Um erro ocorreu no servidor.";

    return Results.Problem(
        title: "Erro interno no servidor",
        detail: detail,
        statusCode: 500
    );
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();

