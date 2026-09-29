# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Thiết lập biến môi trường
ENV ASPNETCORE_ENVIRONMENT=Development

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Project4.csproj", "."]
RUN dotnet restore "./Project4.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Project4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Project4.csproj" -c Release -o /app/publish

# Use the base image for the runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "./Project4.dll"]
