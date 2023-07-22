# Use the official .NET 6 SDK image as the base image for building
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the .csproj files for all projects and restore dependencies
# COPY *.csproj ./
COPY CustomerApiTest/*.csproj ./CustomerApiTest/
COPY CustomerApiTest.DataAccess/*.csproj ./CustomerApiTest.DataAccess/
COPY CustomerApiTest.Exceptions/*.csproj ./CustomerApiTest.Exceptions/
COPY CustomerApiTest.Mediators/*.csproj ./CustomerApiTest.Mediators/
COPY CustomerApiTest.Models/*.csproj ./CustomerApiTest.Models/
COPY CustomerApiTest.sln ./CustomerApiTest.sln
# Add other class libraries as needed

RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port the application will run on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "CustomerApiTest.dll"]