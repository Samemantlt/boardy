using Boardy.Domain.Core.Events;
using TheLiar.Api.Domain.Repositories;
using TheLiar.Api.Extensions;
using TheLiar.Api.SignalR;
using TheLiar.Api.UseCases.Commands;
using TheLiar.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLazy<GameHub>();
builder.Services.RegisterPolymorphicNotificationHandler<IPublicEvent, PublicEventSender>(ServiceLifetime.Singleton);

builder.Services.AddSingleton<IRoomRepository, RoomRepository>();


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateRoom.Request>();
    cfg.RegisterServicesFromAssemblyContaining<PublicEventSender>();
    cfg.RegisterServicesFromAssemblyContaining<InvokeHandler>();
    cfg.AddBehavior<ConcurrentRoomPipelineBehaviour, ConcurrentRoomPipelineBehaviour>(ServiceLifetime.Singleton);
});

// builder.Services.AddSingleton<PublicEventSender>();
builder.Services.AddSingleton<GameHub>();

builder.Services.AddSignalR(opts =>
{
    opts.EnableDetailedErrors = true;
}).AddJsonProtocol(opts =>
{
    opts.PayloadSerializerOptions.IgnoreReadOnlyProperties = false;
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapHub<GameHub>("/hub");
app.MapControllers();

app.MapReverseProxy();

app.Run();