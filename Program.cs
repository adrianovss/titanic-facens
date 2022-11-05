using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("Users"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.MapGet("/", () => "Users Web API (with EF Core In Memory)");


app.UseSwaggerUI();

app.MapGet("/users", async (AppDbContext dbContext) =>
    await dbContext.Users.ToListAsync());

app.MapGet("/users/{id}", async (int id, AppDbContext dbContext) =>
    await dbContext.Users.FirstOrDefaultAsync(a => a.Id == id));

app.MapPost("/users", async (User user, AppDbContext dbContext) =>
{
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    return user;
});

app.MapPut("/users/{id}", async (int id, User user, AppDbContext dbContext) =>
{
    dbContext.Entry(user).State = EntityState.Modified;
    await dbContext.SaveChangesAsync();

    return user;
});

app.MapDelete("/users/{id}", async (int id, AppDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(a => a.Id == id);

    if (user != null)
    {
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();
    }

    return;
});

app.Run();

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; }
}