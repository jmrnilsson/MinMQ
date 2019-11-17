FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
WORKDIR /src
COPY ./service-kestrel/ ./
RUN (cd Service-Kestrel; dotnet publish "Service-Kestrel" -c Release -o /app)

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-buster-slim AS mmq
WORKDIR /app
EXPOSE 3000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Service-Kestrel.dll"]