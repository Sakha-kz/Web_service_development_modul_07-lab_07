using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BankProductsAPI.Services;

namespace BankProductsAPI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class BasicAuthenticationAttribute : Attribute, IAuthorizationFilter
{
    private const string Realm = "BankProductsAPI";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Пропуск для анонимных методов
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
        {
            return;
        }

        // 1. Получение HTTP-заголовка Authorization
        var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            ReturnUnauthorized(context, "Заголовок Authorization отсутствует.");
            return;
        }

        // 2. Проверка схемы Basic
        if (!AuthenticationHeaderValue.TryParse(authHeader, out var headerValue) ||
            !string.Equals(headerValue.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(headerValue.Parameter))
        {
            ReturnUnauthorized(context, "Ожидается базовая аутентификация: Authorization: Basic <base64_credentials>");
            return;
        }

        string credentials;
        try
        {
            // 3. Декодирование Base64 с обработкой некорректного формата
            var bytes = Convert.FromBase64String(headerValue.Parameter);
            credentials = Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            ReturnUnauthorized(context, "Некорректный формат строки Base64.");
            return;
        }

        // 4. Разделение на логин и пароль
        var separator = credentials.IndexOf(':');
        if (separator == -1)
        {
            ReturnUnauthorized(context, "Неверный формат учетных данных. Ожидается: username:password");
            return;
        }

        var username = credentials.Substring(0, separator);
        var password = credentials.Substring(separator + 1);

        // 5. Проверка учетных данных через UserService
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var user = userService.ValidateUser(username, password);

        if (user == null)
        {
            ReturnUnauthorized(context, "Неверные учетные данные пользователя.");
            return;
        }

        // 6. Формирование списка Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // 7. Создание ClaimsIdentity и ClaimsPrincipal
        var identity = new ClaimsIdentity(claims, "Basic");
        var principal = new ClaimsPrincipal(identity);

        // 8. Установка текущего пользователя в HttpContext.User
        context.HttpContext.User = principal;
    }

    private static void ReturnUnauthorized(AuthorizationFilterContext context, string message)
    {
        context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
        context.Result = new UnauthorizedObjectResult(new
        {
            statusCode = StatusCodes.Status401Unauthorized,
            error = "Unauthorized",
            message = message
        });
    }
}
