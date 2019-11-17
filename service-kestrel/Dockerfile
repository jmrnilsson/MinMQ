FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
WORKDIR /src
COPY ./Service-Kestrel/Service-Kestrel .
RUN ls -1
RUN dotnet build "Service-Kestrel.csproj"
RUN dotnet publish "Service-Kestrel.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-buster-slim AS mmq
WORKDIR /app
EXPOSE 9000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Service-Kestrel.dll"]
# ENTRYPOINT ["bash"]