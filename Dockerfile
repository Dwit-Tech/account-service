#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DwitTech.AccountService.WebApi/DwitTech.AccountService.WebApi.csproj", "DwitTech.AccountService.WebApi/"]
COPY . .
WORKDIR "src/DwitTech.AccountService.WebApi"
RUN dotnet restore "DwitTech.AccountService.WebApi.csproj"
RUN dotnet build "DwitTech.AccountService.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DwitTech.AccountService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DwitTech.AccountService.WebApi.dll"]