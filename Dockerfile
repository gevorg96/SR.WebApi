FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY SmartRetail.App.Web/SmartRetail.App.Web.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish SmartRetail.App.sln -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /out
COPY --from=build-env /out .
CMD dotnet SmartRetail.App.Web.dll