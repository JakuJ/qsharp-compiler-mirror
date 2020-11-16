FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
ENV NUGET_VERSION=0.12.20100504
WORKDIR /source

# Bootstrap the repository, filling templates etc.
COPY . .
RUN pwsh bootstrap.ps1

# Restore Q# language server dependencies
WORKDIR src/QsCompiler/ServerRunner
RUN dotnet restore

# Build the Q# language server
RUN dotnet build -c release --no-restore

# Publish the executable
FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:3.1

# Specify the entrypoint to the app
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ServerRunner.dll"]
