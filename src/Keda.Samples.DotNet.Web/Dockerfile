# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Keda.Samples.DotNet.Web/Keda.Samples.DotNet.Web.csproj", "Keda.Samples.DotNet.Web/"]
COPY ["Keda.Samples.Dotnet.Contracts/Keda.Samples.Dotnet.Contracts.csproj", "Keda.Samples.Dotnet.Contracts/"]
RUN dotnet restore "./Keda.Samples.DotNet.Web/Keda.Samples.DotNet.Web.csproj"
COPY . .
WORKDIR "/src/Keda.Samples.DotNet.Web"
RUN dotnet build "./Keda.Samples.DotNet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Keda.Samples.DotNet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keda.Samples.DotNet.Web.dll"]