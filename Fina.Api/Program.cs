
using Fina.Api;
using Fina.Api.Common.Api;
using Fina.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration();
builder.addDataContexts();
builder.AddCrossOrigin();
builder.addDocumentation();
builder.AddServices();



var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.ConfigureDevEnvironment();


app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapEndPoints();

app.Run();


