# Stage 1: Build base image
FROM ubuntu:latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: Build application
FROM ubuntu:latest AS build
WORKDIR /src
COPY ["Songerr/Songerr.csproj", "Songerr/"]
RUN apt update && apt install -y dotnet-sdk-7.0 && \
    dotnet restore "Songerr/Songerr.csproj"
COPY . .
WORKDIR "/src/Songerr"
RUN dotnet build "Songerr.csproj" -c Release -o /app/build

# Stage 3: Publish application
FROM build AS publish
RUN dotnet publish "Songerr.csproj" -c Release -o /app

# Stage 4: Combine stages
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# Install yt-dlp
RUN apt update && apt install -y curl && \
    curl https://raw.githubusercontent.com/yt-dlp/yt-dlp/master/setup/linux/install.sh | bash

ENTRYPOINT ["dotnet", "Songerr.dll"]
