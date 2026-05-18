using FINALPROJECT.Data;
using FINALPROJECT.Models;
using FINALPROJECT.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<FlavorlyDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity
builder.Services.AddIdentity<User, IdentityRole>(opts =>
{
    opts.Password.RequireDigit           = false;
    opts.Password.RequiredLength         = 4;
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
app.UseStaticFiles();
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

    await db.Database.EnsureCreatedAsync();

    // Admin user
    if (await userMgr.FindByEmailAsync("admin@flavorly.com") == null)
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

        // Sample chef user
        var chef = new User
        {
            UserName    = "chef@flavorly.com",
            Email       = "chef@flavorly.com",
            DisplayName = "Maria Chen",
            Bio         = "Professional pastry chef with 15 years of experience.",
            Location    = "New York, NY",
            AvatarUrl   = "/images/avatar2.jpg",
            CoverImageUrl = "/images/cover2.jpg",
            CreatedAt   = DateTime.UtcNow
        };
        await userMgr.CreateAsync(chef, "Chef123!");

        var adminId = admin.Id;
        var chefId  = chef.Id;

        // Recipes
        var recipes = new List<Recipe>
        {
            new() {
                Title = "Classic Eggs Benedict", Description = "A brunch staple — poached eggs on toasted English muffins with hollandaise.",
                HeroImageUrl = "/images/recipe1.jpg", PrepTime = 15, CookTime = 20, Servings = 2,
                Difficulty = Difficulty.Medium, Category = RecipeCategory.BreakfastBrunch,
                IsVegetarian = true, CaloriesPerServing = 480, Protein = 22, Fat = 30, Carbs = 28,
                AuthorId = adminId, CreatedAt = DateTime.UtcNow.AddDays(-10),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="2 English muffins, split and toasted" },
                    new() { OrderIndex=1, Text="4 large eggs" },
                    new() { OrderIndex=2, Text="4 slices Canadian bacon" },
                    new() { OrderIndex=3, Text="2 tbsp white vinegar" },
                    new() { OrderIndex=4, Text="3 egg yolks" },
                    new() { OrderIndex=5, Text="1/2 cup unsalted butter, melted" },
                    new() { OrderIndex=6, Text="1 tbsp lemon juice" },
                    new() { OrderIndex=7, Text="Salt and cayenne pepper to taste" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Make Hollandaise", Description="Whisk yolks and lemon juice in a bowl over simmering water. Slowly drizzle in butter while whisking constantly. Season and keep warm." },
                    new() { StepNumber=2, Title="Poach the Eggs", Description="Bring water with vinegar to a gentle simmer. Crack each egg into a small cup and slide gently into water. Cook 3–4 min." },
                    new() { StepNumber=3, Title="Assemble & Serve", Description="Top each muffin half with bacon, a poached egg, and a generous drizzle of hollandaise. Serve immediately." }
                }
            },
            new() {
                Title = "Spaghetti Carbonara", Description = "Authentic Roman pasta with eggs, pecorino, guanciale and black pepper.",
                HeroImageUrl = "/images/recipe2.jpg", PrepTime = 10, CookTime = 20, Servings = 4,
                Difficulty = Difficulty.Medium, Category = RecipeCategory.MainDishes,
                IsVegetarian = false, CaloriesPerServing = 620, Protein = 28, Fat = 24, Carbs = 72,
                AuthorId = chefId, CreatedAt = DateTime.UtcNow.AddDays(-7),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="400g spaghetti" },
                    new() { OrderIndex=1, Text="200g guanciale or pancetta" },
                    new() { OrderIndex=2, Text="4 large eggs + 2 yolks" },
                    new() { OrderIndex=3, Text="100g Pecorino Romano, grated" },
                    new() { OrderIndex=4, Text="Freshly ground black pepper" },
                    new() { OrderIndex=5, Text="Salt for pasta water" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Cook Pasta", Description="Boil pasta in well-salted water until al dente. Reserve 1 cup pasta water." },
                    new() { StepNumber=2, Title="Render Guanciale", Description="Cook guanciale in a skillet over medium heat until crispy. Remove from heat." },
                    new() { StepNumber=3, Title="Make Sauce", Description="Whisk eggs, yolks, and cheese with pepper. Toss hot pasta with guanciale, add egg mixture off heat, add pasta water to emulsify." }
                }
            },
            new() {
                Title = "Chocolate Lava Cake", Description = "Decadent molten chocolate cakes with a gooey center — ready in 25 min.",
                HeroImageUrl = "/images/recipe3.jpg", PrepTime = 10, CookTime = 12, Servings = 4,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.Desserts,
                IsVegetarian = true, CaloriesPerServing = 390, Protein = 7, Fat = 22, Carbs = 44,
                AuthorId = adminId, CreatedAt = DateTime.UtcNow.AddDays(-5),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="170g dark chocolate (70%)" },
                    new() { OrderIndex=1, Text="115g unsalted butter" },
                    new() { OrderIndex=2, Text="2 whole eggs + 2 yolks" },
                    new() { OrderIndex=3, Text="60g powdered sugar" },
                    new() { OrderIndex=4, Text="2 tbsp all-purpose flour" },
                    new() { OrderIndex=5, Text="Butter and cocoa powder for ramekins" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Prep Ramekins", Description="Butter 4 ramekins and dust with cocoa. Preheat oven to 220°C (425°F)." },
                    new() { StepNumber=2, Title="Melt Chocolate", Description="Melt chocolate and butter together over a double boiler. Cool slightly." },
                    new() { StepNumber=3, Title="Mix & Bake", Description="Whisk eggs, yolks, and sugar. Stir into chocolate. Fold in flour. Fill ramekins and bake 10–12 min. Edges set, center jiggly." }
                }
            },
            new() {
                Title = "Avocado Toast with Poached Egg", Description = "The ultimate healthy breakfast — creamy avocado on sourdough topped with a perfect egg.",
                HeroImageUrl = "/images/recipe4.jpg", PrepTime = 5, CookTime = 10, Servings = 2,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.BreakfastBrunch,
                IsVegetarian = true, IsVegan = false, IsGlutenFree = false,
                CaloriesPerServing = 320, Protein = 14, Fat = 18, Carbs = 28,
                AuthorId = chefId, CreatedAt = DateTime.UtcNow.AddDays(-3),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="2 slices sourdough bread" },
                    new() { OrderIndex=1, Text="1 ripe avocado" },
                    new() { OrderIndex=2, Text="2 eggs" },
                    new() { OrderIndex=3, Text="Red pepper flakes, salt, lemon juice" },
                    new() { OrderIndex=4, Text="Microgreens or fresh herbs for garnish" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Toast Bread", Description="Toast sourdough until golden and crisp." },
                    new() { StepNumber=2, Title="Mash Avocado", Description="Mash avocado with lemon juice, salt, and red pepper flakes." },
                    new() { StepNumber=3, Title="Poach & Assemble", Description="Poach eggs in simmering water 3 min. Spread avocado on toast, top with egg and garnish." }
                }
            },
            new() {
                Title = "Hummus & Pita Chips", Description = "Silky smooth homemade hummus served with crispy baked pita chips.",
                HeroImageUrl = "/images/recipe5.jpg", PrepTime = 15, CookTime = 15, Servings = 6,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.SnacksApps,
                IsVegetarian = true, IsVegan = true, IsGlutenFree = false,
                CaloriesPerServing = 210, Protein = 8, Fat = 10, Carbs = 24,
                AuthorId = adminId, CreatedAt = DateTime.UtcNow.AddDays(-2),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="400g canned chickpeas, drained" },
                    new() { OrderIndex=1, Text="3 tbsp tahini" },
                    new() { OrderIndex=2, Text="2 tbsp olive oil + more for drizzling" },
                    new() { OrderIndex=3, Text="1 lemon, juiced" },
                    new() { OrderIndex=4, Text="1 garlic clove" },
                    new() { OrderIndex=5, Text="Ice water, paprika, salt" },
                    new() { OrderIndex=6, Text="4 pita breads" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Blend Hummus", Description="Process chickpeas, tahini, lemon, garlic and salt. Drizzle in ice water until silky. Adjust seasoning." },
                    new() { StepNumber=2, Title="Make Pita Chips", Description="Cut pitas into triangles, brush with oil, bake at 190°C for 12–15 min until crisp." },
                    new() { StepNumber=3, Title="Serve", Description="Plate hummus with a swirl, drizzle with olive oil and sprinkle paprika. Serve with chips." }
                }
            },
            new() {
                Title = "Beef Tacos", Description = "Juicy seasoned ground beef in warm tortillas topped with fresh salsa and cheese.",
                HeroImageUrl = "/images/recipe6.jpg", PrepTime = 15, CookTime = 15, Servings = 4,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.MainDishes,
                IsVegetarian = false, CaloriesPerServing = 450, Protein = 28, Fat = 22, Carbs = 34,
                AuthorId = chefId, CreatedAt = DateTime.UtcNow.AddDays(-1),
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="500g ground beef" },
                    new() { OrderIndex=1, Text="8 small flour or corn tortillas" },
                    new() { OrderIndex=2, Text="1 tsp each: cumin, chili powder, paprika, garlic powder" },
                    new() { OrderIndex=3, Text="Shredded cheese, salsa, sour cream, lettuce" },
                    new() { OrderIndex=4, Text="Salt, pepper, oil" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Season Beef", Description="Cook beef in a skillet over medium-high heat, breaking up clumps. Drain fat. Add spices and 1/4 cup water. Simmer 3 min." },
                    new() { StepNumber=2, Title="Warm Tortillas", Description="Heat tortillas in a dry pan or directly over a gas flame for 30 seconds each side." },
                    new() { StepNumber=3, Title="Assemble", Description="Fill tortillas with beef mixture. Top with cheese, salsa, sour cream and shredded lettuce." }
                }
            },
            new() {
                Title = "Vegan Buddha Bowl", Description = "A nourishing bowl of roasted veggies, quinoa, and tahini dressing.",
                HeroImageUrl = "/images/recipe7.jpg", PrepTime = 20, CookTime = 30, Servings = 2,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.MainDishes,
                IsVegetarian = true, IsVegan = true, IsGlutenFree = true,
                CaloriesPerServing = 480, Protein = 16, Fat = 18, Carbs = 62,
                AuthorId = adminId, CreatedAt = DateTime.UtcNow,
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="1 cup quinoa, rinsed" },
                    new() { OrderIndex=1, Text="1 can chickpeas, drained and roasted" },
                    new() { OrderIndex=2, Text="2 cups broccoli florets" },
                    new() { OrderIndex=3, Text="1 large sweet potato, diced" },
                    new() { OrderIndex=4, Text="2 tbsp tahini, lemon, garlic, maple syrup" },
                    new() { OrderIndex=5, Text="Avocado, cherry tomatoes, cucumber" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Roast Vegetables", Description="Toss chickpeas, broccoli, and sweet potato with oil and spices. Roast at 200°C for 25–30 min." },
                    new() { StepNumber=2, Title="Cook Quinoa", Description="Simmer quinoa in 2 cups water for 15 min. Fluff with a fork." },
                    new() { StepNumber=3, Title="Make Dressing & Assemble", Description="Whisk tahini, lemon, garlic, maple syrup, and water. Bowl up quinoa, top with veggies and drizzle dressing." }
                }
            },
            new() {
                Title = "Banana Pancakes", Description = "Fluffy 3-ingredient banana pancakes — naturally sweet and gluten-free.",
                HeroImageUrl = "/images/recipe8.jpg", PrepTime = 5, CookTime = 15, Servings = 2,
                Difficulty = Difficulty.Easy, Category = RecipeCategory.BreakfastBrunch,
                IsVegetarian = true, IsGlutenFree = true,
                CaloriesPerServing = 240, Protein = 10, Fat = 8, Carbs = 32,
                AuthorId = chefId, CreatedAt = DateTime.UtcNow,
                Ingredients = new List<RecipeIngredient> {
                    new() { OrderIndex=0, Text="2 ripe bananas, mashed" },
                    new() { OrderIndex=1, Text="2 large eggs" },
                    new() { OrderIndex=2, Text="1/4 tsp cinnamon" },
                    new() { OrderIndex=3, Text="Coconut oil for pan" },
                    new() { OrderIndex=4, Text="Fresh berries and maple syrup to serve" }
                },
                Steps = new List<RecipeStep> {
                    new() { StepNumber=1, Title="Mix Batter", Description="Mash bananas until smooth. Whisk in eggs and cinnamon to form a thin batter." },
                    new() { StepNumber=2, Title="Cook Pancakes", Description="Heat coconut oil in a non-stick pan over medium heat. Pour 2 tbsp batter per pancake. Cook 2 min each side." },
                    new() { StepNumber=3, Title="Serve", Description="Stack and serve with fresh berries and a drizzle of maple syrup." }
                }
            }
        };

        db.Recipes.AddRange(recipes);
        await db.SaveChangesAsync();

        // Reviews
        var reviews = new List<Review>
        {
            new() { RecipeId=1, AuthorId=chefId,  Rating=5, Comment="Absolutely perfect hollandaise every time!", CreatedAt=DateTime.UtcNow.AddDays(-8), HelpfulCount=12 },
            new() { RecipeId=1, AuthorId=adminId, Rating=4, Comment="Great recipe, I added a bit more lemon juice.", CreatedAt=DateTime.UtcNow.AddDays(-6), HelpfulCount=5 },
            new() { RecipeId=2, AuthorId=adminId, Rating=5, Comment="Authentic and delicious. Don't skip the guanciale!", CreatedAt=DateTime.UtcNow.AddDays(-4), HelpfulCount=20 },
            new() { RecipeId=3, AuthorId=chefId,  Rating=5, Comment="My dinner guests were amazed. So simple!", CreatedAt=DateTime.UtcNow.AddDays(-3), HelpfulCount=8 },
            new() { RecipeId=4, AuthorId=adminId, Rating=4, Comment="Love this healthy twist on breakfast.", CreatedAt=DateTime.UtcNow.AddDays(-2), HelpfulCount=4 },
            new() { RecipeId=5, AuthorId=chefId,  Rating=5, Comment="Best hummus I've made at home.", CreatedAt=DateTime.UtcNow.AddDays(-1), HelpfulCount=15 }
        };
        db.Reviews.AddRange(reviews);

        // Videos
        var videos = new List<Video>
        {
            new() {
                Title="Mastering French Pastry Techniques", Description="Learn the fundamentals of classic French pastry from croissants to éclairs.",
                ThumbnailUrl="/images/video1.jpg", VideoUrl="", Duration="18:32", ViewCount=24500,
                Category="Baking", Tags="pastry,french,croissants,baking", AuthorId=chefId,
                CreatedAt=DateTime.UtcNow.AddDays(-14), IsFeatured=true,
                Ingredients="500g bread flour\n300ml whole milk\n10g salt\n50g sugar\n10g instant yeast\n250g cold butter",
                Instructions="1. Mix flour, milk, salt, sugar and yeast into a dough.\n2. Laminate with cold butter through folding.\n3. Shape croissants and proof.\n4. Bake at 200°C for 18 minutes."
            },
            new() {
                Title="Quick 15-Minute Stir Fry", Description="A vibrant, healthy vegetable stir fry you can make any weeknight.",
                ThumbnailUrl="/images/video2.jpg", VideoUrl="", Duration="12:15", ViewCount=18200,
                Category="Vegetarian", Tags="stirfry,quick,vegan,weeknight", AuthorId=adminId,
                CreatedAt=DateTime.UtcNow.AddDays(-10), IsFeatured=false,
                Ingredients="Mixed vegetables\nSoy sauce\nGarlic\nGinger\nSesame oil\nCornstarch",
                Instructions="1. Heat wok over high heat.\n2. Add oil and aromatics.\n3. Add veggies in order of cooking time.\n4. Sauce and toss."
            },
            new() {
                Title="Italian Pizza from Scratch", Description="Traditional Neapolitan pizza dough and tomato sauce made the right way.",
                ThumbnailUrl="/images/video3.jpg", VideoUrl="", Duration="24:40", ViewCount=31000,
                Category="Italian Cuisine", Tags="pizza,italian,neapolitan,dough", AuthorId=chefId,
                CreatedAt=DateTime.UtcNow.AddDays(-5), IsFeatured=false,
                Ingredients="00 flour\nWater\nSalt\nYeast\nSan Marzano tomatoes\nFresh mozzarella\nBasil",
                Instructions="1. Mix dough and cold ferment 24h.\n2. Crush tomatoes for sauce.\n3. Stretch dough by hand.\n4. Top and bake in very hot oven."
            },
            new() {
                Title="Decadent Chocolate Desserts", Description="Three showstopping chocolate desserts for special occasions.",
                ThumbnailUrl="/images/video4.jpg", VideoUrl="", Duration="31:05", ViewCount=15800,
                Category="Desserts", Tags="chocolate,dessert,mousse,cake", AuthorId=adminId,
                CreatedAt=DateTime.UtcNow.AddDays(-2), IsFeatured=false,
                Ingredients="Dark chocolate\nHeavy cream\nButter\nEggs\nSugar\nFlour",
                Instructions="1. Temper chocolate.\n2. Make mousse base.\n3. Assemble layered cake.\n4. Glaze and decorate."
            }
        };
        db.Videos.AddRange(videos);

        // Tips
        var tips = new List<Tip>
        {
            new() { Title="Always rest your meat", Content="After cooking steak or roast, rest it for at least 5–10 minutes before cutting. This allows juices to redistribute and keeps it moist.", AuthorId=adminId, LikeCount=234, CreatedAt=DateTime.UtcNow.AddDays(-20) },
            new() { Title="Salt pasta water generously", Content="Your pasta water should taste like the sea. Add a generous handful of salt once it boils — this is your only chance to season the pasta itself.", AuthorId=chefId, LikeCount=189, CreatedAt=DateTime.UtcNow.AddDays(-15) },
            new() { Title="Mise en place — prep first", Content="Professional chefs always prep all ingredients before cooking. Having everything measured and ready prevents mistakes and speeds up cooking.", AuthorId=adminId, LikeCount=312, CreatedAt=DateTime.UtcNow.AddDays(-10) },
            new() { Title="Use room temperature eggs", Content="Take eggs out of the fridge 30 minutes before baking. Room temperature eggs emulsify better and create a more even, fluffy texture.", AuthorId=chefId, LikeCount=156, CreatedAt=DateTime.UtcNow.AddDays(-5) }
        };
        db.Tips.AddRange(tips);

        // Collections
        var collections = new List<Collection>
        {
            new() { Name="Sunday Brunch Favorites", Description="Our most-loved brunch recipes perfect for a lazy weekend morning.", CoverImageUrl="/images/recipe1.jpg", OwnerId=adminId, CreatedAt=DateTime.UtcNow.AddDays(-30) },
            new() { Name="Quick Weeknight Dinners", Description="Delicious dinners that come together in 30 minutes or less.", CoverImageUrl="/images/recipe6.jpg", OwnerId=chefId, CreatedAt=DateTime.UtcNow.AddDays(-20) },
            new() { Name="Healthy Vegan Meals", Description="Plant-based recipes that are full of flavor and nutrition.", CoverImageUrl="/images/recipe7.jpg", OwnerId=adminId, CreatedAt=DateTime.UtcNow.AddDays(-10) }
        };
        db.Collections.AddRange(collections);
        await db.SaveChangesAsync();

        // Add some recipes to collections
        db.CollectionRecipes.AddRange(
            new CollectionRecipe { CollectionId=1, RecipeId=1, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=1, RecipeId=4, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=1, RecipeId=8, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=2, RecipeId=2, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=2, RecipeId=6, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=3, RecipeId=5, AddedAt=DateTime.UtcNow },
            new CollectionRecipe { CollectionId=3, RecipeId=7, AddedAt=DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }
}
