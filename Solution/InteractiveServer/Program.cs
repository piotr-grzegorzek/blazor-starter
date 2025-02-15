using ElectronNET.API;
using InteractiveServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElectron();
builder.WebHost.UseElectron(args);
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseEnvironment("Development");
}
if (HybridSupport.IsElectronActive)
{
    // Open the Electron-Window here
    await Task.Run(async () =>
    {
        BrowserWindow window = await Electron.WindowManager.CreateWindowAsync();
        window.OnClosed += () => Electron.App.Quit();
    });
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Set up db context
string? connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseFirebird(connectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Apply migrations on startup
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext();
dbContext.Database.Migrate();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<InteractiveServer.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
