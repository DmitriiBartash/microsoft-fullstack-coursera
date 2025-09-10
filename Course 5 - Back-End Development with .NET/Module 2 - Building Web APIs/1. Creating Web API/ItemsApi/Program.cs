using ItemsApi.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Items API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Items API v1");
    });
}

app.UseHttpsRedirection();

var items = new List<Item>();
var nextId = 1;

app.MapGet("/", () => "Welcome to the Simple Web API (NET 9)!");

var itemsApi = app.MapGroup("/items").WithOpenApi();

itemsApi.MapGet("/", () => items);

itemsApi.MapGet("/{id}", (int id) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

itemsApi.MapPost("/", (Item newItem) =>
{
    newItem.Id = nextId++;
    items.Add(newItem);
    return Results.Created($"/items/{newItem.Id}", newItem);
});

itemsApi.MapPut("/{id}", (int id, Item updatedItem) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    return Results.NoContent();
});

itemsApi.MapDelete("/{id}", (int id) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    if (item is null) return Results.NotFound();

    items.Remove(item);
    return Results.NoContent();
});

app.Run();
