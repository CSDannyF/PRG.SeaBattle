using Microsoft.EntityFrameworkCore;
using PRG.EVA01.SeaBattle.Models;
using PRG.EVA01.SeaBattle.Services;
using PRG.EVA01.SeaBattle.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SeaBattleDbContext>(options => options.UseSqlServer(connectionstring));
builder.Services.AddTransient<ISeaBattleService, SeaBattleService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//  name: "throwBomb",
//  pattern: "{controller=SeaBattle}/{action=ThrowBomb}/{letter}/{number}/{id}");

app.Run();
