FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY SmartRetail.App.Web.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish --output /app/ --configuration Release

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app
COPY --from=build-env /app .
CMD dotnet SmartRetail.App.Web.dll