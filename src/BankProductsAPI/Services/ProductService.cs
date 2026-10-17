using BankProductsAPI.Models;

namespace BankProductsAPI.Services;

public interface IProductService
{
    IEnumerable<BankProduct> GetAll();
    BankProduct? GetById(int id);
    BankProduct Add(BankProduct product);
    bool Update(int id, BankProduct product);
    bool Delete(int id);
}

public class ProductService : IProductService
{
    private readonly List<BankProduct> _products = new()
    {
        new BankProduct
        {
            Id = 1,
            Name = "Депозит 'Накопительный'",
            Description = "Срочный вклад с возможностью пополнения и ежемесячной капитализацией процентов",
            InterestRate = 14.5m,
            IsActive = true
        },
        new BankProduct
        {
            Id = 2,
            Name = "Ипотека 'Доступное жилье'",
            Description = "Ипотечный заем на первичное и вторичное жилье с первоначальным взносом от 20%",
            InterestRate = 9.5m,
            IsActive = true
        },
        new BankProduct
        {
            Id = 3,
            Name = "Кредитная карта 'Gold Cashback'",
            Description = "Возобновляемый кредитный лимит с беспроцентным льготным периодом до 55 дней и кешбэком до 10%",
            InterestRate = 21.0m,
            IsActive = true
        },
        new BankProduct
        {
            Id = 4,
            Name = "Автокредит 'Prime'",
            Description = "Целевое финансирование покупки нового автомобиля в автосалонах-партнерах",
            InterestRate = 16.2m,
            IsActive = true
        },
        new BankProduct
        {
            Id = 5,
            Name = "Вклад 'Пенсионный плюс'",
            Description = "Специальный сберегательный депозит с повышенной ставкой для клиентов пенсионного возраста",
            InterestRate = 15.0m,
            IsActive = true
        }
    };

    public IEnumerable<BankProduct> GetAll() => _products;

    public BankProduct? GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public BankProduct Add(BankProduct product)
    {
        var newId = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
        product.Id = newId;
        _products.Add(product);
        return product;
    }

    public bool Update(int id, BankProduct product)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.InterestRate = product.InterestRate;
        existing.IsActive = product.IsActive;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null) return false;

        return _products.Remove(existing);
    }
}
