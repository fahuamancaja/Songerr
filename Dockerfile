# Stage 1: Build base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5002

# Install prerequisites
RUN apt-get update && apt-get install -y \
    python3 \
    python3-venv \
    python3-pip \
    ffmpeg \
    && rm -rf /var/lib/apt/lists/*

# Create and activate virtual environment, then install yt-dlp
RUN python3 -m venv /venv \
    && /venv/bin/pip install yt-dlp

# Ensure yt-dlp is executable
RUN chmod +x /venv/bin/yt-dlp && chmod -R 755 /app

ENV PATH="/venv/bin:$PATH"

# Stage 2: Build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Songerr.Application/Songerr.Application.csproj", "Songerr.Application/"]
COPY ["Songerr.Domain/Songerr.Domain.csproj", "Songerr.Domain/"]
COPY ["Songerr.Infrastructure/Songerr.Infrastructure.csproj", "Songerr.Infrastructure/"]
RUN dotnet restore "Songerr.Application/Songerr.Application.csproj"

COPY . .
WORKDIR "/src/Songerr.Application"
RUN dotnet build "Songerr.Application.csproj" -c Release -o /app/build

# Stage 3: Publish application
FROM build AS publish
RUN dotnet publish "Songerr.Application.csproj" -c Release -o /app

# Stage 4: Combine stages
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Songerr.Application.dll"]