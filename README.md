# Reunião IA

Sistema local para **transcrição automática de reuniões e geração de atas utilizando Inteligência Artificial**.

O projeto utiliza **.NET 8**, **Whisper.net** para reconhecimento de voz e **Ollama + Qwen** para transformar a transcrição em uma ata estruturada.

A proposta é executar todo o processamento **localmente**, evitando o envio dos áudios e transcrições para serviços externos.

---

## 🚀 Objetivo

O objetivo do projeto é permitir que um usuário envie o áudio de uma reunião e receba:

1. A transcrição completa da reunião;
2. Um resumo;
3. Os participantes mencionados;
4. Os assuntos discutidos;
5. As decisões tomadas;
6. As tarefas definidas;
7. Os responsáveis pelas tarefas;
8. Os prazos mencionados;
9. As pendências.

Fluxo planejado:

```text
┌─────────────────┐
│ Arquivo de      │
│ áudio da reunião│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ ASP.NET Core    │
│ Web API         │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Conversão áudio │
│ para 16 kHz     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Whisper.net     │
│ Speech-to-Text  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Transcrição     │
│ da reunião      │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Ollama + Qwen   │
│ geração da ata  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Ata estruturada │
└─────────────────┘
```

---

## 🛠️ Tecnologias

### Backend

* .NET 8
* ASP.NET Core Web API
* C#
* Swagger / OpenAPI

### Inteligência Artificial

* Whisper.net 1.9.1
* Whisper GGML Base
* Ollama
* Qwen3

### Processamento de áudio

* NAudio 2.4.0
* Conversão
