using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Infrastructure.Repositories.Dummy;
using Verum.Infrastructure.Supabase;
using Verum.Web.Filters;
using Verum.Web.Middleware;
using Verum.Web.Services;
using Verum.Web.ViewEngines;

// Formato de moneda consistente ($1,234) sin importar la cultura del servidor.
var verumCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = verumCulture;
CultureInfo.DefaultThreadCurrentUICulture = verumCulture;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<RequireSupabaseAuthFilter>();
    options.Filters.Add<JsonCsrfFilter>();
});
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
// Ningun origen externo esta permitido a proposito: la app solo se usa desde
// si misma. Declarado explicito en vez de dejarlo como una ausencia de config.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationExpanders.Add(new FeatureViewLocationExpander());
});

// Supabase
builder.Services.Configure<SupabaseClientOptions>(builder.Configuration.GetSection("Supabase"));
var supabaseOptions = builder.Configuration.GetSection("Supabase").Get<SupabaseClientOptions>() ?? new SupabaseClientOptions();
builder.Services.AddSingleton(supabaseOptions);
builder.Services.AddScoped(_ => SupabaseClientFactory.Create(supabaseOptions));
builder.Services.AddScoped<ICurrentUserService, SupabaseCurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentBusinessService, CookieCurrentBusinessService>();

// Repositorios: reales contra Supabase, o dummy en memoria segun configuracion
if (supabaseOptions.UseDummyData)
{
    builder.Services.AddScoped<IAccountRepository, DummyAccountRepository>();
    builder.Services.AddScoped<IIncomeRepository, DummyIncomeRepository>();
    builder.Services.AddScoped<IExpenseRepository, DummyExpenseRepository>();
    builder.Services.AddScoped<ICommitmentRepository, DummyCommitmentRepository>();
    builder.Services.AddScoped<IGoalRepository, DummyGoalRepository>();
    builder.Services.AddScoped<IDebtRepository, DummyDebtRepository>();
    builder.Services.AddScoped<ICreditAccountRepository, DummyCreditAccountRepository>();
    builder.Services.AddScoped<IBusinessRepository, DummyBusinessRepository>();
    builder.Services.AddScoped<IGoalImageStorage, DummyGoalImageStorage>();
}
else
{
    builder.Services.AddScoped<IAccountRepository, SupabaseAccountRepository>();
    builder.Services.AddScoped<IIncomeRepository, SupabaseIncomeRepository>();
    builder.Services.AddScoped<IExpenseRepository, SupabaseExpenseRepository>();
    builder.Services.AddScoped<ICommitmentRepository, SupabaseCommitmentRepository>();
    builder.Services.AddScoped<IGoalRepository, SupabaseGoalRepository>();
    builder.Services.AddScoped<IDebtRepository, SupabaseDebtRepository>();
    builder.Services.AddScoped<ICreditAccountRepository, SupabaseCreditAccountRepository>();
    builder.Services.AddScoped<IBusinessRepository, SupabaseBusinessRepository>();
    builder.Services.AddScoped<ISaleRepository, SupabaseSaleRepository>();
    builder.Services.AddScoped<IBusinessExpenseRepository, SupabaseBusinessExpenseRepository>();
    builder.Services.AddScoped<IReceivableRepository, SupabaseReceivableRepository>();
    builder.Services.AddScoped<IPayableRepository, SupabasePayableRepository>();
    builder.Services.AddScoped<IBusinessGoalRepository, SupabaseBusinessGoalRepository>();
    builder.Services.AddScoped<ICostRepository, SupabaseCostRepository>();
    builder.Services.AddScoped<ITaxRepository, SupabaseTaxRepository>();
    builder.Services.AddScoped<IInvestmentRepository, SupabaseInvestmentRepository>();
    builder.Services.AddScoped<IRecurringIncomeRepository, SupabaseRecurringIncomeRepository>();
    builder.Services.AddScoped<IBusinessAccountRepository, SupabaseBusinessAccountRepository>();
    builder.Services.AddScoped<ITransferRepository, SupabaseTransferRepository>();
    builder.Services.AddScoped<IGoalImageStorage, SupabaseGoalImageStorage>();
}

// Servicios de aplicacion
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IRecurringIncomeService, RecurringIncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<ICommitmentService, CommitmentService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IDebtService, DebtService>();
builder.Services.AddScoped<ICreditAccountService, CreditAccountService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IBusinessAccountService, BusinessAccountService>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IBusinessExpenseService, BusinessExpenseService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IPayableService, PayableService>();
builder.Services.AddScoped<IBusinessGoalService, BusinessGoalService>();
builder.Services.AddScoped<ICostService, CostService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IInvestmentService, InvestmentService>();
builder.Services.AddScoped<ISimulationService, SimulationService>();
builder.Services.AddScoped<IAnalysisService, AnalysisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/error/{0}");

// Headers de seguridad en toda respuesta: nunca se permite que la app se
// embeba en un iframe ajeno, nunca se adivina el content-type de un archivo
// subido por el usuario, y los scripts/estilos solo cargan desde origenes
// conocidos (self + Google Fonts + el bucket de Supabase Storage).
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Frame-Options"] = "DENY";
    headers["X-Content-Type-Options"] = "nosniff";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Permissions-Policy"] = "geolocation=(), camera=(), microphone=()";
    headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https://*.supabase.co; " +
        "connect-src 'self'; " +
        "object-src 'none'; " +
        "frame-ancestors 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self';";
    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseMiddleware<SupabaseSessionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
