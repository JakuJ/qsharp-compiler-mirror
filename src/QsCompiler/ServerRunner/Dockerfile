FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /source

# Restore Q# language server dependencies
COPY qsharp-compiler-mirror/ qsharp-compiler-mirror/
WORKDIR /source/qsharp-compiler-mirror/src/QsCompiler/ServerRunner
RUN dotnet restore

# Build the Q# language server
RUN dotnet build -c release --no-restore

# Publish the executable
FROM build AS publish
RUN dotnet publish -c release --no-build -o /app

# Final image
FROM mcr.microsoft.com/dotnet/sdk:3.1
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ServerRunner.dll"]
