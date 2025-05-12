using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDBContext>(options =>

{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDB"));
});
builder.Services.AddIdentity<IdentityUserNew, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();


/*catch(Exception ex)
{
    string dirpath = @"D:\\Love Kashyap\\Projects\\ObjectOP\\FileHandling\\log";
string fullpath = Path.Combine(dirpath + DateTime.Now.ToString("ddMMyyyy"));
if (!File.Exists(fullpath))
{
    File.Create(fullpath);
    File.AppendAllText(fullpath, DateTime.Now.ToString("HHmmss") + ex.Message + "\n");
    File.AppendAllText(fullpath, DateTime.Now.ToString("HHmmss") + ex.StackTrace + "\n");
    Console.WriteLine("New Log File Created " + "\n");

}
else
{
    File.AppendAllText(fullpath, DateTime.Now.ToString("HHmmss") + ex.Message + "\n");
    File.AppendAllText(fullpath, DateTime.Now.ToString("HHmmss") + ex.StackTrace + "\n");
    Console.WriteLine(" Log File Updated " + "\n");

}

}*/