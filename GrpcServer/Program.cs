using System.Text;
using GrpcServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<SessionStorage>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // укзывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = false,
            // строка, представляющая издателя
            ValidIssuer = "MyAuthServer",
 
            // будет ли валидироваться потребитель токена
            ValidateAudience = false,
            // установка потребителя токена
            ValidAudience = "MyAuthClient",
            // будет ли валидироваться время существования
            ValidateLifetime = false,
 
            // установка ключа безопасности
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysupersecret_secretkey!123sdfdsdsfsdfdsfsfdfds")),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = false,
        };
    });
builder.Services.AddAuthorization();

const string corsPolicy = "_corsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        policy  =>
        {
            policy.WithOrigins("https://localhost:5001",
                    "http://localhost:5000")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ForecastService>().EnableGrpcWeb();
app.MapGrpcService<ChatService>().EnableGrpcWeb();
app.MapGrpcService<AuthorizationService>().EnableGrpcWeb();
app.UseCors(corsPolicy);
app.UseGrpcWeb();
app.MapGet("/",
    () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();