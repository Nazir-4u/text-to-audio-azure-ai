
using TextToAudio.Services;
//using TextToAudioTtsWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

//Bind config
var cfgSection = builder.Configuration.GetSection("AzureOpenAI");

var azureCfg = new AzureConfig(
    endpoint: cfgSection["Endpoint"]!,
    apiKey: cfgSection["ApiKey"]!,
    ttsDeployment: cfgSection["TtsDeployment"]!,
    apiVersionTts: cfgSection["ApiVersionTts"]!
);

builder.Services.AddSingleton(azureCfg);

builder.Services.AddHttpClient();
builder.Services.AddScoped<AzureTtsService>();
builder.Services.AddControllers();



// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


public record AzureConfig(
    string endpoint,
    string apiKey,
    string ttsDeployment,
    string apiVersionTts
);