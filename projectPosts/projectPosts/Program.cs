
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();

    builder.Services.AddMemoryCache();
    builder.Services.AddHttpClient();
    builder.Services.AddScoped<DAL.IPostsDAL, DAL.PostsDAL>();
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
        pattern: "{controller=PostsController1}/{action=Index}/{id?}");

    app.Run();
