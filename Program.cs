using System;
using System.Collections.Generic;

// Enum for money denominations
public enum Denomination
{
    OneKr = 1,
    FiveKr = 5,
    TenKr = 10,
    TwentyKr = 20,
    FiftyKr = 50,
    HundredKr = 100,
    FiveHundredKr = 500,
    ThousandKr = 1000
}

// Abstract class for products
public abstract class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }

    public abstract string Examine();
    public abstract string Use();
}

// Product types
public class Drink : Product
{
    public string Flavor { get; set; }

    public override string Examine()
    {
        return $"Drink: {Name}, Flavor: {Flavor}, Cost: {Cost} kr";
    }

    public override string Use()
    {
        return $"Enjoy your {Flavor} drink!";
    }
}

public class Snack : Product
{
    public string Type { get; set; }

    public override string Examine()
    {
        return $"Snack: {Name}, Type: {Type}, Cost: {Cost} kr";
    }

    public override string Use()
    {
        return $"Enjoy your {Type} snack!";
    }
}

public class Toy : Product
{
    public string Category { get; set; }

    public override string Examine()
    {
        return $"Toy: {Name}, Category: {Category}, Cost: {Cost} kr";
    }

    public override string Use()
    {
        return $"Play with your {Category} toy!";
    }
}

// Interface for vending machine operations
public interface IVending
{
    Product Purchase(string productId);
    List<string> ShowAll();
    string Details(string productId);
    void InsertMoney(Denomination denomination);
    Dictionary<Denomination, int> EndTransaction();
}

// Vending machine service class implementing IVending interface
public class VendingMachineService : IVending
{
    private List<Product> products;
    private Dictionary<Denomination, int> moneyPool;

    public VendingMachineService()
    {
        products = new List<Product>
        {
            new Drink { Id = "D1", Name = "Soda", Flavor = "Cola", Cost = 20 },
            new Snack { Id = "S1", Name = "Chips", Type = "Potato", Cost = 15 },
            new Toy { Id = "T1", Name = "Robot", Category = "Electronic", Cost = 50 }
        };

        moneyPool = new Dictionary<Denomination, int>();
        foreach (Denomination denomination in Enum.GetValues(typeof(Denomination)))
        {
            moneyPool.Add(denomination, 0);
        }
    }

    public Product Purchase(string productId)
    {
        Product product = products.Find(p => p.Id == productId);
        if (product != null && moneyPool[Denomination.HundredKr] >= product.Cost)
        {
            moneyPool[Denomination.HundredKr] -= product.Cost;
            return product;
        }
        return null;
    }

    public List<string> ShowAll()
    {
        List<string> productInfo = new List<string>();
        foreach (var product in products)
        {
            productInfo.Add($"Id: {product.Id}, Name: {product.Name}, Cost: {product.Cost} kr");
        }
        return productInfo;
    }

    public string Details(string productId)
    {
        Product product = products.Find(p => p.Id == productId);
        return product?.Examine() ?? "Product not found.";
    }

    public void InsertMoney(Denomination denomination)
    {
        moneyPool[denomination]++;
    }

    public Dictionary<Denomination, int> EndTransaction()
    {
        Dictionary<Denomination, int> change = new Dictionary<Denomination, int>();

        foreach (Denomination denomination in Enum.GetValues(typeof(Denomination)).Cast<Denomination>().OrderByDescending(x => x))
        {
            if (moneyPool[denomination] > 0)
            {
                change.Add(denomination, moneyPool[denomination]);
            }
        }

        moneyPool.Clear();
        foreach (Denomination denomination in Enum.GetValues(typeof(Denomination)))
        {
            moneyPool.Add(denomination, 0);
        }

        return change;
    }
}

// Main class for testing
class Program
{
    static void Main(string[] args)
    {
        IVending vendingMachine = new VendingMachineService();
        bool continueShopping = true;

        while (continueShopping)
        {
            Console.WriteLine("Available Products:");
            List<string> productsInfo = vendingMachine.ShowAll();
            foreach (var productInfo in productsInfo)
            {
                Console.WriteLine(productInfo);
            }

            Console.WriteLine("Available Denominations: 1kr, 5kr, 10kr, 20kr, 50kr, 100kr, 500kr, 1000kr");
            Console.Write("Enter the product ID you want to purchase (or 'exit' to end): ");
            string productId = Console.ReadLine();

            if (productId.ToLower() == "exit")
            {
                continueShopping = false;
            }
            else
            {
                Console.Write("Enter the denomination (1kr, 5kr, 10kr, 20kr, 50kr, 100kr, 500kr, 1000kr): ");
                Enum.TryParse(Console.ReadLine(), out Denomination denomination);

                if (Enum.IsDefined(typeof(Denomination), denomination))
                {
                    vendingMachine.InsertMoney(denomination);
                    Product selectedProduct = vendingMachine.Purchase(productId);
                    if (selectedProduct != null)
                    {
                        Console.WriteLine($"Purchased: {selectedProduct.Name}");
                        Console.WriteLine(selectedProduct.Use());
                    }
                    else
                    {
                        Console.WriteLine("Invalid product ID or insufficient funds.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid denomination.");
                }
            }
        }

        Dictionary<Denomination, int> change = vendingMachine.EndTransaction();
        Console.WriteLine("Change:");
        foreach (var denomination in change)
        {
            Console.WriteLine($"{denomination.Key} kr: {denomination.Value} notes");
        }
    }
}
