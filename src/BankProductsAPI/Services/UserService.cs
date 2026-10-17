using BankProductsAPI.Models;

namespace BankProductsAPI.Services;

public interface IUserService
{
    User? ValidateUser(string username, string password);
    IEnumerable<User> GetAll();
}

public class UserService : IUserService
{
    private readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            UserName = "AdminUser",
            Password = "Admin123",
            Email = "admin@bank.kz",
            Roles = new List<string> { "Admin" }
        },
        new User
        {
            Id = 2,
            UserName = "ManagerUser",
            Password = "Manager123",
            Email = "manager@bank.kz",
            Roles = new List<string> { "Manager" }
        },
        new User
        {
            Id = 3,
            UserName = "ClientUser",
            Password = "Client123",
            Email = "client@bank.kz",
            Roles = new List<string> { "Client" }
        }
    };

    public User? ValidateUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }

    public IEnumerable<User> GetAll() => _users;
}
