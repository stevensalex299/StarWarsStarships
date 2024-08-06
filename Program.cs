using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register Starship DB Context
builder.Services.AddDbContext<StarshipContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("StarshipDatabase")));

// Register the StarshipService for API calls
builder.Services.AddHttpClient<StarshipService>();

// Register the DbSeeder service
builder.Services.AddScoped<DbSeeder>();

var app = builder.Build();

// Apply migrations and seed the database
using (var scope = app.Services.CreateScope())
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await dbSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Starships}/{action=Index}/{id?}");

app.Run();
