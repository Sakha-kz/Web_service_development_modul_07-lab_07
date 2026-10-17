using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankProductsAPI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public MyAuthorizeAttribute(params string[] roles)
    {
        _roles = roles ?? Array.Empty<string>();
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // 1. Проверка аутентификации пользователя
        if (user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                statusCode = StatusCodes.Status401Unauthorized,
                error = "Unauthorized",
                message = "Пользователь не аутентифицирован."
            });
            return;
        }

        // 2. Если конкретные роли не заданы, доступ разрешен любому аутентифицированному пользователю
        if (_roles.Length == 0)
        {
            return;
        }

        // 3. Проверка прав доступа через User.IsInRole()
        var hasRequiredRole = _roles.Any(role => user.IsInRole(role));

        if (!hasRequiredRole)
        {
            // 4. Возврат HTTP 403 Forbidden при нехватке полномочий
            context.Result = new ObjectResult(new
            {
                statusCode = StatusCodes.Status403Forbidden,
                error = "Forbidden",
                message = $"Доступ запрещен. Требуется одна из ролей: {string.Join(", ", _roles)}.",
                userRoles = user.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
