using Newtonsoft.Json;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Persistence;

public class JsonDataStore
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Formatting = Formatting.Indented
    };

    private readonly string _usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
    private readonly string _productsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json");
    private readonly string _cartsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "carts.json");
    private readonly string _walletsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wallets.json");

    public void LoadUsers(AppDb db)
    {
        if (!File.Exists(_usersPath))
            return;

        var json = File.ReadAllText(_usersPath);
        var users = JsonConvert.DeserializeObject<List<User>>(json, Settings);
        if (users != null)
        {
            db.Users.Clear();
            db.Users.AddRange(users);
        }
    }

    public void SaveUsers(AppDb db)
    {
        var json = JsonConvert.SerializeObject(db.Users, Settings);
        File.WriteAllText(_usersPath, json);
    }

    public void LoadProducts(AppDb db)
    {
        if (!File.Exists(_productsPath))
            return;

        var json = File.ReadAllText(_productsPath);
        var products = JsonConvert.DeserializeObject<List<Product>>(json, Settings);
        if (products != null)
        {
            db.Products.Clear();
            db.Products.AddRange(products);
        }
    }

    public void SaveProducts(AppDb db)
    {
        var json = JsonConvert.SerializeObject(db.Products, Settings);
        File.WriteAllText(_productsPath, json);
    }

    public void LoadCarts(AppDb db)
    {
        if (!File.Exists(_cartsPath))
            return;

        var json = File.ReadAllText(_cartsPath);
        var carts = JsonConvert.DeserializeObject<List<Cart>>(json, Settings);
        if (carts != null)
        {
            db.Carts.Clear();
            db.Carts.AddRange(carts);
        }
    }

    public void SaveCarts(AppDb db)
    {
        var json = JsonConvert.SerializeObject(db.Carts, Settings);
        File.WriteAllText(_cartsPath, json);
    }

    public void LoadWallets(AppDb db)
    {
        if (!File.Exists(_walletsPath))
            return;

        var json = File.ReadAllText(_walletsPath);
        var wallets = JsonConvert.DeserializeObject<List<Wallet>>(json, Settings);
        if (wallets != null)
        {
            db.Wallets.Clear();
            db.Wallets.AddRange(wallets);
        }
    }

    public void SaveWallets(AppDb db)
    {
        var json = JsonConvert.SerializeObject(db.Wallets, Settings);
        File.WriteAllText(_walletsPath, json);
    }
}
