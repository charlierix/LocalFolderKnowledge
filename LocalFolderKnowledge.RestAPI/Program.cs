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

// Endpoints
app.MapGet("/listfolders", (IConfiguration config) =>
{
    return ListFolders.List(config["FolderLocation"]);
})
.WithName("GetListFolders");

app.MapPost("/addfolder", (AddFolderRequest request, IConfiguration config) =>
{
    return AddFolder.Add(request, config["FolderLocation"]);
})
.WithName("PostAddFolder");

app.Run();
