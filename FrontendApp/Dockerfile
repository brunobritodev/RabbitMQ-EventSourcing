FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
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