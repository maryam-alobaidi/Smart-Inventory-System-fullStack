var builder = WebApplication.CreateBuilder(args);

// 1. إضافة الخدمات (Add services)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- إضافة هذا الجزء لتعريف سياسة الـ CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
// --------------------------------------------

// إعداد نص الاتصال
string? connectingString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectingString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
Inventory.DataAccess.clsDataAccessSettings.ConnectionString = connectingString;

var app = builder.Build();

// 2. إعداد خط الأنابيب (Configure the HTTP request pipeline)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); // للسماح بالوصول لمجلد wwwroot

// ترتيب الأسطر هنا مهم جداً
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // الآن سيعمل لأننا عرفناه فوق

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();