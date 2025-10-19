using MinimalApiDemo.Models;
using MinimalApiDemo.Services;

namespace MinimalApiDemo.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/tasks")
                       .WithOpenApi()
                       .WithTags("Tasks");

        // GET /tasks
        group.MapGet("/", (TaskService service) =>
            Results.Ok(service.GetAll()))
            .WithName("GetAllTasks")
            .WithDescription("Retrieve all tasks");

        // GET /tasks/{id}
        group.MapGet("/{id:int}", (TaskService service, int id) =>
        {
            var task = service.GetById(id);
            return task is not null ? Results.Ok(task) : Results.NotFound();
        })
        .WithName("GetTaskById")
        .WithDescription("Retrieve a task by its ID");

        // POST /tasks
        group.MapPost("/", (TaskService service, TaskItem newTask) =>
        {
            try
            {
                var created = service.Create(newTask);
                return Results.Created($"/tasks/{created.Id}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(ex.Message);
            }
        })
        .WithName("CreateTask")
        .WithDescription("Create a new task");

        // PUT /tasks/{id}
        group.MapPut("/{id:int}", (TaskService service, int id, TaskItem updated) =>
        {
            var updatedTask = service.Update(id, updated);
            return updatedTask is not null ? Results.Ok(updatedTask) : Results.NotFound();
        })
        .WithName("UpdateTask")
        .WithDescription("Update an existing task");

        // DELETE /tasks/{id}
        group.MapDelete("/{id:int}", (TaskService service, int id) =>
        {
            return service.Delete(id) ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTask")
        .WithDescription("Delete a task by ID");
    }
}
