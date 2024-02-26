using CityInfo.API;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<IValidator<PointOfInterestForCreationDto>, PointOfInterestValidation>();
builder.Services.AddLogging();
builder.Services.AddTransient<ILocalMailService, LocalMailService>();
//builder.Services.AddDbContext<CityInfoContext>( x => x.UseSqlite("Data Source=CityInfo.db"));
builder.Services.AddDbContext<CityInfoContext>();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
//app.MapControllerRoute("default", "{controller}/{action}");

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });


app.Run();