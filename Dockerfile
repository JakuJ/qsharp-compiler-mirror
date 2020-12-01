FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
ENV NUGET_VERSION=0.14.2011120240
WORKDIR /source

# Bootstrap the repository, filling templates etc.
COPY . .
RUN pwsh bootstrap.ps1

# Restore Q# language server dependencies
WORKDIR src/QsCompiler/WebSocketServer
RUN dotnet restore

# Build the Q# language server
RUN dotnet build -c release --no-restore

# Publish the executable
FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:3.1

# Set ASP.NET Core runtime environment
ARG ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}

# Specify the entrypoint to the app
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebSocketServer.dll"]
