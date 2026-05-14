FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY EventDrivenEcommerce.slnx .
COPY src/Payment.Api/Payment.Api.csproj src/Payment.Api/
COPY src/Shared.Contracts/Shared.Contracts.csproj src/Shared.Contracts/
COPY src/Shared.Messaging/Shared.Messaging.csproj src/Shared.Messaging/

RUN dotnet restore src/Payment.Api/Payment.Api.csproj

COPY src/Payment.Api src/Payment.Api
COPY src/Shared.Contracts src/Shared.Contracts
COPY src/Shared.Messaging src/Shared.Messaging

RUN dotnet publish src/Payment.Api/Payment.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Payment.Api.dll"]