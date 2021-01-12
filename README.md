# Language server for the Quantum Explorer

This repository contains code behind the WebSocket-based language server for Q#.
It's built upon a mirror of the [qsharp-compiler](https://github.com/microsoft/qsharp-compiler) repository by Microsoft.
Our code is located mostly at `src/QsCompiler/WebSocketServer`, with some modifications done to the original code as well. 

For documentation on the rest of the code contained in this repository, refer to the [original README](./README_ORIGINAL.md).

# Building

## Requirements

Make sure you have the following installed:

- .NET Core 3.1 SDK and runtime
- Docker (optional)

## Language server

If you want to build locally, you must first execute the `bootstrap.ps1` script at the root of
the repository with the correct value of the `NUGET_VERSION` environment variable (defined in
the [Dockerfile](./Dockerfile)).

```shell
# On MacOS or Linux, pwsh is the Powershell binary
NUGET_VERSION=0.14.2011120240 pwsh bootstrap.ps1

# On Windows, in Powershell
$Env:NUGET_VERSION=0.14.2011120240
bootstrap.ps1

# build and run the Language Server afterwards
cd src/QsCompiler/WebSocketServer
dotnet run
```
By default, the app runs on port `8091`.

The app can also be run inside a Docker container:

```shell
docker build -t language-server .
docker run -p 8091:8091 -t language-server
```


# Deployment

We use two versions of the app deployed at all times to the cloud(s).

## Development

The development version of the app, tracking the `develop` branch is running at https://qexplorer-ls.herokuapp.com. The
deployment happens automatically using a push hook configured in Heroku.

The runtime config involves setting **config vars** as follows:

| Variable | Value |
|---|---|
| ASPNETCORE_ENVIRONMENT | Development |

## Production

The production version of the app, is deployed to an Azure Web App for Containers running
at https://language-server.azurewebsites.net.

The deployment is performed manually by pushing the Docker container (or rather, its build context, the build is remote) to Azure Container Repository (ACR).
Run the following Azure CLI command from the root folder of the repository:

```shell 
az acr build --registry QuantumExplorer --image language-server:<tag> --verbose .
```

where `<tag>` is the docker image tag for the release (e.g. `latest` or `1.0`).

The runtime config involves setting **application settings** as follows:

| Variable | Value |
|---|---|
| ASPNETCORE_ENVIRONMENT | Production |
