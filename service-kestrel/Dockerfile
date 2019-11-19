FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-buster AS build
WORKDIR /src
COPY ./Service-Kestrel/Service-Kestrel .
RUN dotnet build "Service-Kestrel.csproj"
RUN dotnet publish "Service-Kestrel.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-buster-slim AS mmq
# ENV ASPNETCORE_ENVIRONMENT Docker
# ENV ASPNETCORE_URLS "http://0.0.0.0:9000"
ENV ASPNETCORE_ENVIRONMENT "Docker"
ENV ASPNETCORE_URLS "http://0.0.0.0:9000"

WORKDIR /app
EXPOSE 9000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Service-Kestrel.dll"]
# ENTRYPOINT ["bash"]