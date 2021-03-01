FROM mcr.microsoft.com/dotnet/sdk:5.0
ENV ASPNETCORE_URLS="http://+:80/;https://+:443/"

WORKDIR /app

# Restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything and build the solution
COPY . ./
RUN dotnet publish -c Release -o out

ENTRYPOINT [ "dotnet", "out/LeaderboardApi.dll" ]