using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektBlaz.Components;
using ProjektBlaz.Components.Account;
using ProjektBlaz.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Authentication and Authorization services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>(); // Custom Identity user accessor service
builder.Services.AddScoped<IdentityRedirectManager>(); // Handles redirects after login/signup
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // Lock for 10 minutes
    options.Lockout.MaxFailedAccessAttempts = 3; // Lock account after 3 failed attempts
    options.Lockout.AllowedForNewUsers = true; 
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies(); // Add cookie authentication for Identity

// Connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // Ensures Entity Framework uses SQL Server

// Add Database Developer Page Exception Filter (for development)
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register Identity services for ApplicationUser
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>() // Use custom ApplicationDbContext for Identity
    .AddSignInManager() // Adds SignInManager to handle user sign-in
    .AddDefaultTokenProviders(); // Default token providers for Identity (e.g., for email confirmation)

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>(); // No-op email sender for dev, replace for production

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // Add migrations in development (useful during development)
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); // Enforce HTTPS in production for security
}

// Use HTTPS redirection and Anti-forgery
app.UseHttpsRedirection();
app.UseAntiforgery(); // Add anti-forgery support

app.MapStaticAssets(); // Serve static files (e.g., images, CSS, JS)
app.MapRazorComponents<App>()  // Map Razor Components for Blazor app
    .AddInteractiveServerRenderMode(); // Use Interactive Server rendering mode

// Add additional Identity endpoints for account management (login, register, etc.)
app.MapAdditionalIdentityEndpoints();



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next();
});


// Run the application
app.Run();
