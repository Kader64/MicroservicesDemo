var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/", () => "Hello from Service 2!");

app.MapGet("/service1", async (IHttpClientFactory httpClientFactory) =>
{
    // TODO Fix when Service 1 is not loaded yet
    var client = httpClientFactory.CreateClient();
    var response = await client.GetStringAsync("http://service1:8080/");
    return $"Response from Service 1: {response}";
});

app.Run();