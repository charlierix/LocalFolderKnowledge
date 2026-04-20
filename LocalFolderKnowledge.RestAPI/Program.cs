using LocalFolderKnowledge.ClassLib.Implementations;
using LocalFolderKnowledge.ClassLib.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/listfolders", () =>
{
    return ListFolders.List();
})
.WithName("GetListFolders");

app.MapPost("/addfolder", (AddFolderRequest request) =>
{
    return AddFolder.Add(request);
})
.WithName("PostAddFolder");


app.Run();
