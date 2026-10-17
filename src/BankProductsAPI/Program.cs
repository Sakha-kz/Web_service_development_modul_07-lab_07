using Microsoft.OpenApi.Models;
using BankProductsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Добавление контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. Регистрация сервисов приложения в DI
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IProductService, ProductService>();

// 3. Настройка Swagger UI со схемой Basic Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BankProductsAPI - Module 07 Laboratory / SRSP",
        Version = "v1",
        Description = "ASP.NET Core Web API: Управление банковскими продуктами с ролевым доступом (Admin, Manager, Client) через Basic Authentication."
    });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Введите логин и пароль тестового пользователя (AdminUser:Admin123, ManagerUser:Manager123, ClientUser:Client123)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankProductsAPI v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.MapControllers();

app.Run();
