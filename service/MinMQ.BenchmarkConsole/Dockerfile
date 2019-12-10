FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src

# Dockerfile requires to be built with docker-compose to set context to parent directory. This is to apply the projects 
# ./StyleCop.ruleset.

COPY ./MinMQ.BenchmarkConsole .
COPY ./StyleCop.ruleset ../

RUN dotnet build "MinMQ.BenchmarkConsole.csproj"
RUN dotnet publish "MinMQ.BenchmarkConsole.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet AS mmq
ENV ASPNETCORE_ENVIRONMENT "Docker"
ENV REQUEST_PATH "http://mmq-service:9000/send"

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MinMQ.BenchmarkConsole.dll", "3000"]
