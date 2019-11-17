FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
WORKDIR /src
COPY . .
RUN dotnet publish "src.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-buster-slim AS mmq
WORKDIR /app
EXPOSE 3000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "src.dll"]