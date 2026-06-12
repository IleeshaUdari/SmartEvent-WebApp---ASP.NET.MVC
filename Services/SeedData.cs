using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartEventWeb.Areas.Identity.Data;
using SmartEventWeb.Models;

namespace SmartEventWeb.Services
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var db = sp.GetRequiredService<SmartEventWebContext>();
            await db.Database.MigrateAsync();

            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = sp.GetRequiredService<UserManager<IdentityUser>>();

            const string adminRole = "Admin";
            const string adminEmail = "admin@council.lk";
            const string adminPassword = "Admin@12345";

            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, adminPassword);
            }

            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                await userManager.AddToRoleAsync(adminUser, adminRole);

            // Categories
            if (!await db.Categories.AnyAsync())
            {
                db.Categories.AddRange(
                    new Category { Name = "Music" },
                    new Category { Name = "Theatre" },
                    new Category { Name = "Sport" }
                );
                await db.SaveChangesAsync();
            }

            // Venues
            if (!await db.Venues.AnyAsync())
            {
                db.Venues.AddRange(
                    new Venue { Name = "City Hall", Location = "Colombo" },
                    new Venue { Name = "Open Air Theatre", Location = "Kandy" }
                );
                await db.SaveChangesAsync();
            }

            var music = await db.Categories.FirstAsync(x => x.Name == "Music");
            var theatre = await db.Categories.FirstAsync(x => x.Name == "Theatre");

            var colombo = await db.Venues.FirstAsync(v => v.Location == "Colombo");
            var kandy = await db.Venues.FirstAsync(v => v.Location == "Kandy");

            // Events (include 495 sold scenario)
            if (!await db.Events.AnyAsync())
            {
                db.Events.AddRange(
                    new Event
                    {
                        Title = "Council Music Night",
                        EventDate = DateTime.Today.AddDays(7),
                        CategoryId = music.CategoryId,
                        VenueId = colombo.VenueId,
                        Price = 2500,
                        TicketCapacity = 500,
                        TicketsSold = 495
                    },
                    new Event
                    {
                        Title = "Drama Festival",
                        EventDate = DateTime.Today.AddDays(17),
                        CategoryId = theatre.CategoryId,
                        VenueId = kandy.VenueId,
                        Price = 1800,
                        TicketCapacity = 300,
                        TicketsSold = 120
                    }
                );

                await db.SaveChangesAsync();
            }
        }
    }
}
