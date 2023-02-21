var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapGet("/users", () => "Hello World!");

app.Run();