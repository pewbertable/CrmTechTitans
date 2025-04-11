using CrmTechTitans.Data;
using CrmTechTitans.Middleware;
using CrmTechTitans.Models;
using CrmTechTitans.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Get identity options from appsettings.json
var identityOptions = builder.Configuration.GetSection("IdentityOptions");
var requireConfirmedAccount = identityOptions.GetValue<bool>("SignIn:RequireConfirmedAccount", false);
var requireConfirmedEmail = identityOptions.GetValue<bool>("SignIn:RequireConfirmedEmail", false);
var twoFactorEnabled = identityOptions.GetValue<bool>("TwoFactor:Enabled", true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<CrmContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Email Sender
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Register Excel Export Service
builder.Services.AddScoped<ExcelExportService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = requireConfirmedAccount;
        options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
        
        // Enable two-factor authentication
        options.Tokens.AuthenticatorIssuer = "CrmTechTitans";
        options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        
        // Configure password requirements from appsettings.json
        if (identityOptions.GetSection("Password") != null)
        {
            options.Password.RequireDigit = identityOptions.GetValue<bool>("Password:RequireDigit", true);
            options.Password.RequireLowercase = identityOptions.GetValue<bool>("Password:RequireLowercase", true);
            options.Password.RequireUppercase = identityOptions.GetValue<bool>("Password:RequireUppercase", true);
            options.Password.RequireNonAlphanumeric = identityOptions.GetValue<bool>("Password:RequireNonAlphanumeric", true);
            options.Password.RequiredLength = identityOptions.GetValue<int>("Password:RequiredLength", 6);
        }
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add authorization to require login for all pages by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Use our user approval middleware
app.UseUserApprovalCheck();

// Use session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Create database and apply migrations before seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Ensure Identity database exists and has been migrated
        var identityContext = services.GetRequiredService<ApplicationDbContext>();
        identityContext.Database.EnsureCreated();
        
        // Ensure CRM database exists and has been migrated
        var crmContext = services.GetRequiredService<CrmContext>();
        crmContext.Database.EnsureCreated();
        
        // Initialize CRM data
        CrmInitializer.Initialize(serviceProvider: services);
        
        // Initialize Identity roles and users
        await IdentityInitializer.InitializeAsync(services);
        
        // Update all existing users to Approved status
        await CrmTechTitans.Data.Utilities.UserStatusUpdater.UpdateExistingUsersToApproved(services);
        
        Console.WriteLine("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        Console.WriteLine("Error during database initialization: " + ex.Message);
    }
}

app.Run();
