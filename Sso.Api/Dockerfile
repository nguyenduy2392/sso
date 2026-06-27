# BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Sso.Api/Sso.Api.csproj Sso.Api/
COPY Sso.Core/Sso.Core.csproj Sso.Core/

RUN dotnet restore Sso.Api/Sso.Api.csproj

COPY Sso.Api/ Sso.Api/
COPY Sso.Core/ Sso.Core/

RUN dotnet publish Sso.Api/Sso.Api.csproj -c Release -o /app/publish

# RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Sso.Api.dll"]
