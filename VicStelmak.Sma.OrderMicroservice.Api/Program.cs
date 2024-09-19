using VicStelmak.Sma.OrderMicroservice.ApiDataLibrary.Features.Order;
using VicStelmak.Sma.OrderMicroservice.ApiDataLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.ConfigureOrderEndpoints();
app.Run();