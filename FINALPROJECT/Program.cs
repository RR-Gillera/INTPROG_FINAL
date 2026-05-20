using FINALPROJECT.Data;
using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<FlavorlyDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// 2. Identity
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequireDigit           = true;
    opts.Password.RequiredLength         = 6;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase       = false;
    opts.Password.RequireLowercase       = false;
})
.AddEntityFrameworkStores<FlavorlyDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath  = "/account/login";
    opts.LogoutPath = "/account/logout";
});

// 3. Repositories
builder.Services.AddScoped<IRecipeRepository,    RecipeRepository>();
builder.Services.AddScoped<IVideoRepository,     VideoRepository>();
builder.Services.AddScoped<ICollectionRepository,CollectionRepository>();
builder.Services.AddScoped<ITipRepository,       TipRepository>();

// 4. MVC
builder.Services.AddControllersWithViews();

// 5. Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opts =>
{
    opts.IdleTimeout        = TimeSpan.FromMinutes(30);
    opts.Cookie.HttpOnly    = true;
    opts.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
    }
});
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Migrate + Seed
await SeedDatabase(app);

app.Run();

// ─── Seed ─────────────────────────────────────────────────────────────────
static async Task SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db          = scope.ServiceProvider.GetRequiredService<FlavorlyDbContext>();
    var userMgr     = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleMgr     = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();

    // Create Admin role if not exists
    if (!await roleMgr.RoleExistsAsync("Admin"))
    {
        await roleMgr.CreateAsync(new IdentityRole("Admin"));
    }

    // Ensure existing admin user is assigned to Admin role if they exist
    var existingAdmin = await userMgr.FindByEmailAsync("admin@flavorly.com");
    if (existingAdmin != null && !await userMgr.IsInRoleAsync(existingAdmin, "Admin"))
    {
        await userMgr.AddToRoleAsync(existingAdmin, "Admin");
    }

    // Admin user
    if (existingAdmin == null)
    {
        var admin = new User
        {
            UserName    = "admin@flavorly.com",
            Email       = "admin@flavorly.com",
            DisplayName = "Chef Admin",
            Bio         = "The Flavorly team chef and community manager.",
            Location    = "San Francisco, CA",
            AvatarUrl   = "/images/avatar1.jpg",
            CoverImageUrl = "/images/cover1.jpg",
            CreatedAt   = DateTime.UtcNow
        };
        await userMgr.CreateAsync(admin, "Admin123!");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }

    // Ensure a default SystemSettings row exists
    if (!await db.SystemSettings.AnyAsync())
    {
        db.SystemSettings.Add(new SystemSetting
        {
            SiteName = "Flavorly",
            Tagline = "Cook. Share. Inspire.",
            MaintenanceMode = false,
            AutoFlaggingKeywords = false,
            AutoHideReportsThreshold = 5,
            RequireManualApproval = false,
            SmtpHost = "",
            SmtpPort = 587,
            SmtpUsername = "",
            SmtpPassword = ""
        });
        await db.SaveChangesAsync();
    }
}
