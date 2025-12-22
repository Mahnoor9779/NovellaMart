using NovellaMart.Components;
using NovellaMart.Core.BL.Services; // CRITICAL: Import the Service Namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- REGISTER SERVICES ---
// 1. Register CartService so Cart.razor can use it.
//    'Scoped' means one cart instance per user connection.
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<NovellaMart.Core.BL.Services.OrderService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<FlashSaleService>();
builder.Services.AddScoped<FlashSaleCrudService>();



// 2. Add Controller support (since you have Core/BL/Controllers)
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Map the UI Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map the API Controllers (e.g. api/cart)
app.MapControllers();

app.Run();