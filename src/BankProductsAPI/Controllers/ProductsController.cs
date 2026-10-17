using Microsoft.AspNetCore.Mvc;
using BankProductsAPI.Attributes;
using BankProductsAPI.Models;
using BankProductsAPI.Services;

namespace BankProductsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[BasicAuthentication]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Просмотр всех банковских продуктов (Доступ: Admin, Manager, Client)
    /// </summary>
    [HttpGet]
    [MyAuthorize("Admin", "Manager", "Client")]
    public IActionResult GetAll()
    {
        var products = _productService.GetAll();
        return Ok(new
        {
            message = "Список банковских продуктов успешно получен.",
            operatorName = User.Identity?.Name,
            roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value),
            total = products.Count(),
            data = products
        });
    }

    /// <summary>
    /// Просмотр продукта по ID (Доступ: Admin, Manager, Client)
    /// </summary>
    [HttpGet("{id}")]
    [MyAuthorize("Admin", "Manager", "Client")]
    public IActionResult GetById(int id)
    {
        var product = _productService.GetById(id);
        if (product == null)
        {
            return NotFound(new { message = $"Банковский продукт с ID {id} не найден." });
        }

        return Ok(new
        {
            message = $"Продукт #{id} найден.",
            operatorName = User.Identity?.Name,
            data = product
        });
    }

    /// <summary>
    /// Создание нового банковского продукта (Доступ: Admin, Manager)
    /// </summary>
    [HttpPost]
    [MyAuthorize("Admin", "Manager")]
    public IActionResult Create([FromBody] BankProduct product)
    {
        if (product == null || string.IsNullOrWhiteSpace(product.Name))
        {
            return BadRequest(new { message = "Название продукта обязательно." });
        }

        var created = _productService.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
        {
            message = "Банковский продукт успешно добавлен.",
            createdBy = User.Identity?.Name,
            product = created
        });
    }

    /// <summary>
    /// Редактирование продукта (Доступ: Admin, Manager)
    /// </summary>
    [HttpPut("{id}")]
    [MyAuthorize("Admin", "Manager")]
    public IActionResult Update(int id, [FromBody] BankProduct product)
    {
        if (product == null)
        {
            return BadRequest(new { message = "Данные для обновления не переданы." });
        }

        var updated = _productService.Update(id, product);
        if (!updated)
        {
            return NotFound(new { message = $"Продукт с ID {id} не найден для обновления." });
        }

        return Ok(new
        {
            message = $"Продукт #{id} успешно обновлен.",
            updatedBy = User.Identity?.Name,
            product = _productService.GetById(id)
        });
    }

    /// <summary>
    /// Удаление банковского продукта (Доступ: ТОЛЬКО Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [MyAuthorize("Admin")]
    public IActionResult Delete(int id)
    {
        var deleted = _productService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Продукт с ID {id} не найден для удаления." });
        }

        return Ok(new
        {
            message = $"Банковский продукт #{id} успешно удален администратором.",
            deletedBy = User.Identity?.Name
        });
    }
}
