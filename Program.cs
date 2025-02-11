using ElectronNET.API;
using ElectronNET.API.Entities;



var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseElectron(args);
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy1",
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                      });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


// 啟用 Electron
if (HybridSupport.IsElectronActive)
{
    ElectronBootstrap();
}
else
{
    Console.WriteLine("not in Electron env.");
}
var api = app.MapGroup("/api").DisableAntiforgery();


app.UseCors("Policy1");

app.Run();

void ElectronBootstrap()
{
    Task.Run(async () =>
    {
        Console.WriteLine("Electron start");
        var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));


        var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
        {
            Show = false,
            Width = 1280,
            Height = 720,
            WebPreferences = new WebPreferences
            {
                Preload = Path.Combine(projectRoot, "preload.js"),
                NodeIntegration = false,
                ContextIsolation = true,
                WebSecurity = false,
            }
        });

        window.LoadURL("http://localhost:5173");
        //window.LoadURL("file://" + Path.Combine(projectRoot.Replace("\\", "/"), "wwwroot/index.html"));


        window.WebContents.OpenDevTools();
        // 監聽網頁載入完成的事件
        window.WebContents.OnDidFinishLoad += () =>
        {
            Console.WriteLine("Electron window loaded successfully.");
            window.Show(); // 網頁載入完成後顯示視窗

            Electron.IpcMain.Send(window, "back-end", "test");
        };

        

        window.OnClosed += () =>
        {
            Electron.App.Quit(); // 當視窗關閉時退出應用
        };
    });
}