using Microsoft.EntityFrameworkCore;
using ToDoAPI;

var builder = WebApplication.CreateBuilder(args);

// add DI - add service
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// configure pipeline
app.MapGet("/todoitems", async (TodoDb db) =>
                            await db.Todos.ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
                            await db.Todos.FindAsync(id));

app.MapPost("todoitems", async (TodoItem item, TodoDb db) =>
                    {
                        db.Todos.Add(item);  
                        await db.SaveChangesAsync();
                        return Results.Created($"/todoitems/{item.Id}", item);
                     });

app.MapPut("/todoitems/{id}" , async (int id , TodoItem item , TodoDb db) =>
                    {
                        var todo = await db.Todos.FindAsync(id);
                        if(todo == null)
                            return Results.NotFound();
                        todo.Title = item.Title;
                        todo.IsComplete = item.IsComplete;
                        await db.SaveChangesAsync();
                        return Results.NoContent();
                    });

app.MapGet("/", () => "Hello World! Todo App");

app.Run();
