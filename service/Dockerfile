FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src
COPY ./MinMQ.Service .
COPY ./StyleCop.ruleset ../
RUN dotnet build "MinMQ.Service.csproj"
RUN dotnet publish "MinMQ.Service.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet AS mmq
# ENV ASPNETCORE_ENVIRONMENT Docker
# ENV ASPNETCORE_URLS "http://0.0.0.0:9000"
ENV ASPNETCORE_ENVIRONMENT "Docker"
ENV ASPNETCORE_URLS "http://0.0.0.0:9000"

WORKDIR /app
EXPOSE 9000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MinMQ.Service.dll"]
# ENTRYPOINT ["bash"]
