using System.Diagnostics.Eventing.Reader;
using Task_5;
using Task_5.Data_Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<DataGenerator>();
var app = builder.Build();

app.Map("/", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/Frontend/MainPage.html");
});

app.MapGet("api/Data", async (DataGenerator generator, string region, int pageNumber, double errorCount, int seed) =>
{
    List<User> users;
    if (pageNumber > 1) { users = await generator.GenerateUserDataAsync(region, seed, pageNumber, errorCount, pageSize: 10); }
    else { users = await generator.GenerateUserDataAsync(region, seed, pageNumber, errorCount); }
    return Results.Ok(users);
});
app.Run();
