FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY SmartRetail.App.Web/SmartRetail.App.Web.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish SmartRetail.App.sln -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
CMD dotnet SmartRetail.App.Web.dll
