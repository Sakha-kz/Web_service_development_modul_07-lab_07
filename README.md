# Модуль 07. Лабораторная работа / СРСП: BankProductsAPI (Ролевая авторизация банковских продуктов)

Лабораторная работа (СРСП) по дисциплине «Разработка веб-сервисов» (CSE5032).  
Кафедра Программной инженерии, Институт Автоматики и информационных технологий.

---

## 🎯 Цель работы
Изучить механизмы аутентификации и авторизации в ASP.NET Core Web API, освоить создание пользовательских фильтров безопасности (`IAuthorizationFilter`), работу с объектами `ClaimsIdentity` и `ClaimsPrincipal`, а также научиться разграничивать доступ к банковским продуктам на основе ролей пользователей (`Admin`, `Manager`, `Client`).

---

## 🛠 Технологический стек
- **Платформа:** .NET 9.0 (C# 13)
- **Фреймворк:** ASP.NET Core Web API
- **Механизм безопасности:** Custom `BasicAuthenticationAttribute`, `MyAuthorizeAttribute` (`IAuthorizationFilter`)
- **Документация и UI:** Swagger UI (Swashbuckle 6.6.2) с поддержкой схемы `Basic`

---

## 📂 Структура проекта

```text
Web_service_development_modul_07-lab_07/
├── .gitignore
├── BankProductsAPI.sln
├── README.md
├── DESIGN_AND_ANSWERS.md
└── src/
    └── BankProductsAPI/
        ├── Attributes/
        │   ├── BasicAuthenticationAttribute.cs  # Декодирование Base64, валидация через UserService
        │   └── MyAuthorizeAttribute.cs          # Проверка ролей (User.IsInRole)
        ├── Controllers/
        │   └── ProductsController.cs            # CRUD эндпоинты банковских продуктов
        ├── Models/
        │   ├── BankProduct.cs                   # Модель продукта (Id, Name, Description, InterestRate, IsActive)
        │   └── User.cs                          # Модель пользователя (Id, UserName, Password, Email, Roles)
        ├── Services/
        │   ├── IProductService.cs / ProductService.cs # Сервис продуктов (CRUD + 5 начальных вкладов/кредитов)
        │   └── IUserService.cs / UserService.cs       # Хранилище тестовых пользователей
        ├── Program.cs                           # Регистрация сервисов и Swagger Basic Auth
        └── BankProductsAPI.csproj
```

---

## 👥 Роли и матрица прав доступа

| Роль | Логин | Пароль | Разрешённые операции | HTTP-методы |
| :--- | :--- | :--- | :--- | :--- |
| **Admin** | `AdminUser` | `Admin123` | Просмотр, создание, изменение и удаление продуктов | `GET`, `POST`, `PUT`, `DELETE` |
| **Manager** | `ManagerUser` | `Manager123` | Просмотр, создание и изменение продуктов | `GET`, `POST`, `PUT` |
| **Client** | `ClientUser` | `Client123` | Только просмотр продуктов | `GET` |

---

## 📋 Эндпоинты API

| Метод | Маршрут | Разрешённые роли | Описание |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/products` | `Admin`, `Manager`, `Client` | Просмотр всех банковских продуктов |
| `GET` | `/api/products/{id}` | `Admin`, `Manager`, `Client` | Просмотр продукта по ID |
| `POST` | `/api/products` | `Admin`, `Manager` | Создание нового продукта |
| `PUT` | `/api/products/{id}` | `Admin`, `Manager` | Обновление продукта |
| `DELETE` | `/api/products/{id}` | `Admin` *(только)* | Удаление продукта из каталога |

---

## 🚀 Быстрый запуск

### 1. Сборка проекта
```powershell
cd E:\Web_service_development_RK1\week_7\Web_service_development_modul_07-lab_07
dotnet build
```

### 2. Запуск приложения
```powershell
dotnet run --project src\BankProductsAPI
```
После запуска перейдите по адресу: `http://localhost:5000/swagger`. В Swagger UI используйте кнопку **Authorize** для ввода учетных данных.
