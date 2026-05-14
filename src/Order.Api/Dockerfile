FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EventDrivenEcommerce.slnx .
COPY src/Order.Api/Order.Api.csproj src/Order.Api/
COPY src/Shared.Contracts/Shared.Contracts.csproj src/Shared.Contracts/
COPY src/Shared.Messaging/Shared.Messaging.csproj src/Shared.Messaging/

RUN dotnet restore src/Order.Api/Order.Api.csproj

COPY src/Order.Api src/Order.Api
COPY src/Shared.Contracts src/Shared.Contracts
COPY src/Shared.Messaging src/Shared.Messaging

RUN dotnet publish src/Order.Api/Order.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Order.Api.dll"]