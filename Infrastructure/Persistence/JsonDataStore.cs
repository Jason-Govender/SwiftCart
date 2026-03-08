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
}
