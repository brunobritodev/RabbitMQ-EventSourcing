FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["PedidoService/OrderService.csproj", "PedidoService/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "PedidoService/OrderService.csproj"
COPY . .
WORKDIR "/src/PedidoService"
RUN dotnet build "OrderService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OrderService.dll"]