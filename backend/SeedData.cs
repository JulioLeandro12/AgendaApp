using ContactApi.Data;
using ContactApi.Models;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Contacts.Any()) return;

        var list = new Contact[]
        {
            new Contact { Name = "Ana Silva", Email = "ana.silva@example.com", Phone = "+55 81 99999-0001" },
            new Contact { Name = "Bruno Souza", Email = "bruno.souza@example.com", Phone = "+55 81 99999-0002" },
            new Contact { Name = "Carla Lima", Email = "carla.lima@example.com", Phone = "+55 81 99999-0003" }
        };

        context.Contacts.AddRange(list);
        context.SaveChanges();
    }
}
