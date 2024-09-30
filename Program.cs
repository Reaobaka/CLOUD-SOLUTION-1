using RetailStore.Models;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace RetailStore
{
    public class Program
    {
        public static void Main(string[] args)
        {



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
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
            app.Run();
           
        }
    }
}