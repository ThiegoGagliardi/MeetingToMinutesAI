# Meeting to Minutes AI

Sistema de IA para transcrição de reuniões e geração automática de atas utilizando **Whisper**, **Ollama**, **Qwen** e **.NET 8**, com processamento totalmente local.

## 🚀 Tecnologias

* .NET 8
* ASP.NET Core Web API
* C#
* Whisper.net
* Whisper GGML
* NAudio
* Ollama
* Qwen
* Docker
* Docker Compose
* Swagger / OpenAPI

## 🏗️ Arquitetura

O projeto está sendo desenvolvido seguindo princípios de **Clean Architecture** e **DDD**.

```text
MeetingToMinutesAI
│
├── src
│   ├── ReuniaoIA.Api
│   ├── ReuniaoIA.Application
│   ├── ReuniaoIA.Domain
│   └── ReuniaoIA.Infrastructure
│
├── tests
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## 🧠 Como funciona

O fluxo principal da aplicação é:

```text
Áudio da reunião
       │
       ▼
    Whisper
       │
       ▼
   Transcrição
       │
       ▼
     Ollama
       │
       ▼
      Qwen
       │
       ▼
   Ata da reunião
```

Todo o processamento pode ser executado localmente, sem necessidade de enviar o áudio ou a transcrição para serviços externos.

## 🎙️ Whisper

O projeto utiliza o Whisper para realizar a transcrição do áudio.

O modelo utilizado atualmente é:

```text
ggml-base.bin
```

O modelo é baixado automaticamente na primeira execução e armazenado no diretório:

```text
Models/ggml-base.bin
```

O áudio processado pelo Whisper precisa estar em **16 kHz**. O projeto utiliza o NAudio para realizar a conversão quando necessário.

## 🤖 Ollama e Qwen

O Ollama é utilizado para executar o modelo de linguagem localmente.

Modelo utilizado:

```text
qwen3:8b
```

O Qwen recebe a transcrição produzida pelo Whisper e gera uma ata estruturada contendo informações como:

* título da reunião
* resumo
* participantes
* assuntos discutidos
* decisões
* tarefas
* responsáveis
* prazos
* pendências

O sistema é instruído a não inventar informações que não estejam presentes na transcrição.

## 🐳 Docker

O projeto possui suporte a **Docker Compose**, permitindo executar a API e o Ollama em containers separados.

### Subir os containers

Na raiz do projeto:

```bash
docker compose up -d --build
```

Verificar os containers:

```bash
docker compose ps
```

Os serviços serão executados da seguinte forma:

```text
API
http://localhost:5279

Ollama
http://localhost:11434
```

### Baixar o modelo Qwen

Após iniciar os containers, baixe o modelo dentro do container do Ollama:

```bash
docker exec -it reuniaoia-ollama ollama pull qwen3:8b
```

Verificar os modelos instalados:

```bash
docker exec -it reuniaoia-ollama ollama list
```

Testar o modelo:

```bash
docker exec -it reuniaoia-ollama ollama run qwen3:8b
```

### Swagger

Com os containers em execução, acesse:

```text
http://localhost:5279/swagger
```

### Parar os containers

```bash
docker compose down
```

Para parar os containers e também remover os volumes:

```bash
docker compose down -v
```

> **Atenção:** o comando `docker compose down -v` remove os volumes utilizados pelo projeto, incluindo os modelos armazenados pelo Ollama e pelo Whisper.

### Volumes

Os modelos são armazenados em volumes persistentes:

```text
reuniaoia-whisper-models
        ↓
/app/Models
```

```text
reuniaoia-ollama-models
        ↓
/root/.ollama
```

Dessa forma, os modelos não precisam ser baixados novamente sempre que os containers forem recriados.

## ⚙️ Configuração

A configuração do Ollama pode ser definida no `appsettings.json`:

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "qwen3:8b"
  }
}
```

Quando executada dentro do Docker Compose, a API utiliza o nome do serviço Docker:

```text
http://ollama:11434
```

Isso é importante porque `localhost` dentro do container da API aponta para o próprio container da API, e não para o container do Ollama.

## ▶️ Executando sem Docker

Restaurar as dependências:

```bash
dotnet restore
```

Executar a aplicação:

```bash
dotnet run
```

Acessar o Swagger:

```text
http://localhost:5279/swagger
```

O Ollama também precisa estar executando localmente:

```bash
ollama run qwen3:8b
```

## 📡 Endpoint

Endpoint principal:

```http
POST /api/Reunioes/gerar-ata
```

O endpoint recebe um arquivo de áudio utilizando `multipart/form-data`.

Exemplo:

```bash
curl -X POST \
  http://localhost:5279/api/Reunioes/gerar-ata \
  -F "arquivo=@reuniao.wav"
```

## 📄 Resposta

A API retorna a transcrição e a ata gerada:

```json
{
  "transcricao": "Hoje discutimos o planejamento do próximo sprint...",
  "ata": {
    "titulo": "Planejamento do próximo sprint",
    "resumo": "Discussão sobre as atividades do próximo sprint.",
    "participantes": [],
    "assuntosDiscutidos": [],
    "decisoes": [],
    "tarefas": [],
    "pendencias": []
  }
}
```

## 🔒 Processamento local

Um dos objetivos principais do projeto é manter o processamento local.

```text
Áudio
  │
  ▼
Whisper ──────────────┐
  │                   │
  ▼                   │
Transcrição            │
  │                   │
  ▼                   │
Ollama + Qwen          │
  │                   │
  ▼                   │
Ata                    │
                      │
          Tudo local ─┘
```

Nenhum áudio precisa ser enviado para uma API de terceiros.

## 🗺️ Roadmap

* [x] Upload de áudio
* [x] Conversão do áudio para 16 kHz
* [x] Transcrição utilizando Whisper
* [x] Integração com Ollama
* [x] Geração de ata utilizando Qwen
* [x] Docker
* [x] Docker Compose
* [ ] Refatoração completa para Clean Architecture
* [ ] Domain-Driven Design
* [ ] OllamaSharp
* [ ] Geração de JSON estruturado pela IA
* [ ] Entity Framework Core
* [ ] MySQL
* [ ] Persistência das reuniões
* [ ] Frontend React
* [ ] Autenticação
* [ ] Histórico de reuniões
* [ ] Suporte a outros formatos de áudio
* [ ] Suporte a GPU NVIDIA
* [ ] Testes unitários
* [ ] Testes de integração
* [ ] CI/CD

## 💡 Objetivo do projeto

Este projeto foi desenvolvido como estudo prático de **Inteligência Artificial aplicada ao desenvolvimento de software**, explorando a integração entre modelos de reconhecimento de voz, modelos de linguagem e aplicações .NET.

Além disso, o projeto busca aplicar conceitos de:

* Clean Architecture
* Domain-Driven Design
* SOLID
* APIs REST
* Inteligência Artificial generativa
* processamento local de IA
* Docker
* Entity Framework Core
* MySQL
* React

## 📜 Licença

Este projeto está disponível para fins educacionais e de estudo.

