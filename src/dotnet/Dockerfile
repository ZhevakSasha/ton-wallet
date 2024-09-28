# Установим .NET SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Копируем .sln файл и .csproj файлы для каждого проекта
COPY TonWalletTest.sln ./
COPY TonWallet.Domain/TonWallet.Domain.csproj ./TonWallet.Domain/
COPY TonWallet.Infrastructure/TonWallet.Infrastructure.csproj ./TonWallet.Infrastructure/
COPY TonWalletApi/TonWalletApi.csproj ./TonWalletApi/
COPY TonWalletBot/TonWalletBot.csproj ./TonWalletBot/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем остальные файлы и компилируем проект
COPY . ./
RUN dotnet publish -c Release -o out

# Запускаем приложение с использованием базового образа ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Открываем порт и запускаем приложение
EXPOSE 80
ENTRYPOINT ["dotnet", "TonWalletApi.dll"]