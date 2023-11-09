var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new {controller="Login", action="Index"});

app.MapControllerRoute(
    name: "cadastrar",
    pattern: "cadastre-se",
    defaults: new { controller = "Login", action = "FormCadastro" });

app.MapControllerRoute(
    name: "recuperar senha",
    pattern: "credenciais",
    defaults: new { controller = "Login", action = "FormRecuperar" });

app.MapControllerRoute(
    name: "Dados Pessoais",
    pattern: "dados-pessoais",
    defaults: new { controller = "Login", action = "FormAlterar" });

app.MapControllerRoute(
    name: "Dashboard",
    pattern: "dashboard",
    defaults: new { controller = "Content", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
