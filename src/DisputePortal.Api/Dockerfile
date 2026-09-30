# build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.sln* ./
COPY src/DisputePortal.Domain/*.csproj src/DisputePortal.Domain/
COPY src/DisputePortal.Infrastructure/*.csproj src/DisputePortal.Infrastructure/
COPY src/DisputePortal.Api/*.csproj src/DisputePortal.Api/
RUN dotnet restore src/DisputePortal.Api/DisputePortal.Api.csproj
COPY . .
RUN dotnet publish src/DisputePortal.Api/DisputePortal.Api.csproj -c Release -o /app

# run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "DisputePortal.Api.dll"]