using SwiftCart.Infrastructure.Data;
using SwiftCart.Infrastructure.Persistence;

AppDb db = new AppDb();
JsonDataStore jsonDataStore = new JsonDataStore();

jsonDataStore.LoadUsers(db);
SeedData.SeedUsersIfEmpty(db);

try
{
    Console.WriteLine("SwiftCart");
}
finally
{
    jsonDataStore.SaveUsers(db);
}
