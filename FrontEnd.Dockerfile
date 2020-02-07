FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FrontendApp/FrontendApp.csproj", "FrontendApp/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "FrontendApp/FrontendApp.csproj"
COPY . .
WORKDIR "/src/FrontendApp"
RUN dotnet build "FrontendApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FrontendApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FrontendApp.dll"]