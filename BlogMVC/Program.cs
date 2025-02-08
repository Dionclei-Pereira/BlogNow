using BlogMVC.Data;
using BlogMVC.Interfaces;
using BlogMVC.Models;
using BlogMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = "server=localhost;userid=root;password=12345678;database=blognow";
builder.Services.AddDbContext<BlogNowContext>(options => options.UseMySql(connection, ServerVersion.AutoDetect(connection)));
builder.Services.AddScoped<SeedingDB>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false; 
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequiredLength = 6; 
    options.Password.RequiredUniqueChars = 0;
    options.User.AllowedUserNameCharacters = null;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<BlogNowContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Index";
    options.AccessDeniedPath = "/Account/Error";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();
//roles
var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

string admin = "Admin";
if (!await roleManager.RoleExistsAsync(admin)) {
    await roleManager.CreateAsync(new IdentityRole(admin));
}
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

var user = await userManager.FindByEmailAsync("Beah2323@gmail.com");
if (user != null) {
    await userManager.AddToRoleAsync(user, admin);
}

//


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=VerifyLogin}/{id?}");


app.Run();
