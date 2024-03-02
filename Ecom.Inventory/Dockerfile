#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ecom.Inventory.csproj", "."]

ARG GH_OWNER
ARG GH_PAT

RUN dotnet nuget add source --username USERNAME --password "$GH_PAT" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/$GH_OWNER/index.json"

RUN dotnet restore "./Ecom.Inventory.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Ecom.Inventory.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecom.Inventory.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecom.Inventory.dll"]