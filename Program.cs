using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// ✅ Connection String
var connectionString = builder.Configuration.GetConnectionString("SmartEventDB")
    ?? throw new InvalidOperationException("Connection string 'SmartEventWebContextConnection' not found.");

builder.Services.AddDbContext<SmartEventWebContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Identity + Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<SmartEventWebContext>();

// ✅ Google Authentication (External Login)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    });

var app = builder.Build();

// ✅ Apply migrations + seed data after app is built
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;

    var db = sp.GetRequiredService<SmartEventWebContext>();
    db.Database.Migrate();

    // seed categories/venues/events/reviews
    await SmartEventWeb.Services.SeedData.SeedAsync(sp);

    // seed admin + Admin role
    await SeedAdminAsync(sp);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


// ✅ Admin seed method (for Inquiry Management)
static async Task SeedAdminAsync(IServiceProvider sp)
{
    const string adminRole = "Admin";
    const string adminEmail = "admin@smart.com";
    const string adminPassword = "Admin@12345";

    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (!result.Succeeded) return;
    }

    if (!await userManager.IsInRoleAsync(admin, adminRole))
        await userManager.AddToRoleAsync(admin, adminRole);
}
