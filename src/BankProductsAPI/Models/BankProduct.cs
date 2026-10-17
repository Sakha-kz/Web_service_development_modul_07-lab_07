namespace BankProductsAPI.Models;

public class BankProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal InterestRate { get; set; } // Процентная ставка, например 14.5%
    public bool IsActive { get; set; }
}
