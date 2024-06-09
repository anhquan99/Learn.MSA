using MSA.ProductService.Entities;
using MSA.Common.MongoDB;
using MSA.Common.PostgresMassTransit.MassTransit;
using MSA.Common.Security.Authentication;
using MSA.Common.Security.Authorization;
using MSA.Common.Contracts.Settings;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddMongo()
                .AddRepositories<Product>("product")
                .AddMassTransitWithRabbitMQ()
                .AddMSAAuthentication()
                .AddMSAAuthorization(opt =>
                {
                    opt.AddPolicy("read_access", policy =>
                    {
                        policy.RequireClaim("scope", "productapi.read");
                    });
                });

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var srvUrlsSetting = builder.Configuration.GetSection(nameof(ServiceUrlsSetting)).Get<ServiceUrlsSetting>();
builder.Services.AddSwaggerGen(options =>
{
    var scheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{srvUrlsSetting.IndentityServiceUrl}/connect/authorize"),
                TokenUrl = new Uri($"{srvUrlsSetting.IndentityServiceUrl}/connext/token"),
                Scopes = new Dictionary<string, string>{
                    {"productapi.read", "Access read operations"},
                    {"productapi.write", "Access write operations"}
                }
            }
        },
        Type = SecuritySchemeType.OAuth2
    };
    options.AddSecurityDefinition("OAuth", scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{Id = "OAuth", Type = ReferenceType.SecurityScheme}
            },
            new List<string>{}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("product-swagger");
        options.OAuthScopes("profile", "openid");
    });
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();