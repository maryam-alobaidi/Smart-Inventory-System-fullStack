FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# ننسخ ملفات csproj فقط أولاً (لتحسين الأداء عبر Docker cache)
COPY Inventory.API/Inventory.API.csproj Inventory.API/
COPY Inventory.BusinessLogic/Inventory.BusinessLogic.csproj Inventory.BusinessLogic/
COPY Inventory.DataAccess/Inventory.DataAccess.csproj Inventory.DataAccess/
COPY Inventory.DTOs.Model/Inventory.DTOs.Model.csproj Inventory.DTOs.Model/
RUN dotnet restore Inventory.API/Inventory.API.csproj

# الحين ننسخ باقي الملفات كاملة ونبني
COPY . .
RUN dotnet publish Inventory.API/Inventory.API.csproj -c Release -o /app/out

# مرحلة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.API.dll"]