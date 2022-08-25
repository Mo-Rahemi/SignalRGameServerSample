using SignalRGameServerSample.Hubs;
using SignalRGameServerSample.Models;

var builder = WebApplication.CreateBuilder(args);

//Disable object to json change property name with camel case principle
builder.Services.AddSignalR().AddJsonProtocol(o => { o.PayloadSerializerOptions.PropertyNamingPolicy = null; });
//--------
//Disable Cross Orgine protection
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        x =>
        {
            x.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));
//-------------
builder.Services.AddSingleton<GameUpdate>();
builder.Services.AddSingleton<Helper>();

var app = builder.Build();

app.UseCors("CorsPolicy");
app.MapHub<GameHub>("/GameHub");

app.Run();
