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
}
