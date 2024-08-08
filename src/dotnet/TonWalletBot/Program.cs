using System;
using Telegram.Bot;
using TonWalletBot;
using TonWalletBot.Commands;
using TonWalletBot.Services;
using Microsoft.EntityFrameworkCore;
using TonWallet.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://warm-hopefully-donkey.ngrok-free.app", "https://localhost:4444")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddHostedService<ConfigureWebhook>();


builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddScoped<BaseCommand, StartCommand>();

builder.Services.AddScoped<ICommandExecutor, CommandExecutor>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();


app.MapControllers();

app.Run();
