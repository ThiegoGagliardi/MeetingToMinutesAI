# 1. Imagem base de runtime para a execução final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root

# Instala bibliotecas nativas de C++ e suporte a OpenMP exigidos pelo whisper.cpp
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgomp1 \
    libstdc++6 \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*
    
WORKDIR /app
EXPOSE 8080

# 2. Imagem do SDK para compilação e restauração
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura dependências
COPY ["ReuniaoIA.Api/ReuniaoIA.Api.csproj", "ReuniaoIA.Api/"]
RUN dotnet restore "ReuniaoIA.Api/ReuniaoIA.Api.csproj"

# Copia o código-fonte restante e compila
COPY . .
WORKDIR "/src/ReuniaoIA.Api"
RUN dotnet build "ReuniaoIA.Api.csproj" -c Release -o /app/build

# 3. Estágio de publicação (Herda do 'build' sem criar dependências circulares)
FROM build AS publish
RUN dotnet publish "ReuniaoIA.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Estágio final de execução
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReuniaoIA.Api.dll"]
