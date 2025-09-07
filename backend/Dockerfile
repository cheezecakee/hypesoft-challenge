FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Hypesoft.API/*.csproj ./Hypesoft.API/
COPY Hypesoft.Application/*.csproj ./Hypesoft.Application/
COPY Hypesoft.Domain/*.csproj ./Hypesoft.Domain/
COPY Hypesoft.Infrastructure/*.csproj ./Hypesoft.Infrastructure/

RUN dotnet restore ./Hypesoft.API/Hypesoft.API.csproj

COPY . .

# Build in development mode
RUN dotnet build ./Hypesoft.API/Hypesoft.API.csproj -c Debug -o /app/build

# Publish for dev (no trimming, dev environment)
RUN dotnet publish ./Hypesoft.API/Hypesoft.API.csproj -c Debug -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Set environment to Development
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5113

# Expose dev port
EXPOSE 5113

# Run the app
ENTRYPOINT ["dotnet", "Hypesoft.API.dll"]

