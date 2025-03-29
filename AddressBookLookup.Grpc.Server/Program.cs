using AddressBookLookup.Grpc.Application.Interfaces;
using AddressBookLookup.Grpc.Application.Services;
using AddressBookLookup.Grpc.Domain.Interfaces;
using AddressBookLookup.Grpc.Infrastructure.Data;
using AddressBookLookup.Grpc.Infrastructure.Repositories;
using AddressBookLookup.Grpc.Server.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAddressBookService, AddressBookService>();
builder.Services.AddScoped<IAddressBookRepository, AddressBookRepository>();
builder.Services.AddHostedService<RandomLogBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Drop and recreate the database to ensure fresh data
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<AddressBookGrpcService>();

await app.RunAsync();
