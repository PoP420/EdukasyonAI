# EdukasyonAI 🎓

**Offline-First AI Learning Platform for Filipino Public School Students**

Digital education gaps persist due to connectivity and device limitations. EdukasyonAI solves this by providing an AI-powered, offline-first learning experience aligned with the DepEd K-12 curriculum.

---

## 📋 Overview

| Layer | Technology |
|-------|-----------|
| Backend API | ASP.NET Core 8 + Clean Architecture |
| Mobile App | .NET MAUI (Android 8.0+ / iOS 15+) |
| AI Engine | NVIDIA Nemotron Nano (4-bit INT8 quantized) |
| Local DB | SQLite (offline-first) |
| Cloud DB | PostgreSQL (background sync) |
| Admin Web | Blazor Web App |

---

## 🏗️ Solution Structure

```
EdukasyonAI/
├── src/
│   ├── EdukasyonAI.Domain.Shared/      # Enums, constants (GradeLevel, Subject, Language …)
│   ├── EdukasyonAI.Domain/             # Entities, repository interfaces, domain services
│   ├── EdukasyonAI.Application.Contracts/ # DTOs, application service interfaces
│   ├── EdukasyonAI.Application/        # Application services (Student, Course, AI, Auth)
│   ├── EdukasyonAI.Infrastructure/     # EF Core repositories, Nemotron service, sync
│   ├── EdukasyonAI.HttpApi/            # REST API controllers (Auth, Students, Courses, AI)
│   ├── EdukasyonAI.HttpApi.Host/       # ASP.NET Core host + Swagger + JWT config
│   ├── EdukasyonAI.DbMigrator/         # EF Core migrations runner
│   ├── EdukasyonAI.Maui/               # MAUI mobile app (MVVM, SQLite, offline sync)
│   └── EdukasyonAI.Web/                # Blazor admin + teacher dashboard
└── test/
    ├── EdukasyonAI.Domain.Tests/       # Domain service unit tests
    └── EdukasyonAI.Application.Tests/  # Application service unit tests (Moq)
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL 14+ (for cloud database)
- NVIDIA API key (for Nemotron cloud inference)

### 1. Configure Secrets (never commit real secrets)

Use **dotnet user-secrets** (recommended for local development):

```bash
cd src/EdukasyonAI.HttpApi.Host

dotnet user-secrets set "Jwt:Key" "your-strong-secret-key-at-least-32-characters"
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Database=edukasyon_ai_dev;Username=postgres;Password=yourpassword"
dotnet user-secrets set "Nemotron:ApiKey" "your-nvidia-api-key"
```

For production, set environment variables:

```bash
export EdukasyonAI__Jwt__Key="your-strong-production-key"
export EdukasyonAI__Nemotron__ApiKey="your-nvidia-api-key"
export EdukasyonAI__ConnectionStrings__PostgreSQL="Host=...;Database=...;Username=...;Password=..."
```

> ⚠️ **Never commit real secrets to source control.**
> The values in `appsettings.json` are placeholder strings only.

### 2. Run Database Migrations

```bash
cd src/EdukasyonAI.DbMigrator
dotnet run
```

### 3. Start the API

```bash
cd src/EdukasyonAI.HttpApi.Host
dotnet run
# Swagger UI: https://localhost:7131
```

### 4. Start the Admin Dashboard

```bash
cd src/EdukasyonAI.Web
dotnet run
# Open: https://localhost:7052/teacher/dashboard
```

### 5. Run Tests

```bash
dotnet test EdukasyonAI.slnx
```

---

## 📱 MAUI Mobile App

The `EdukasyonAI.Maui` project targets Android 8.0+ (API 26+) and iOS 15+.

To build for Android:

```bash
# Install MAUI workload first:
dotnet workload install maui-android

cd src/EdukasyonAI.Maui
dotnet build -f net8.0-android
```

**Key Features:**
- Offline-first with SQLite local storage
- CommunityToolkit.Mvvm (MVVM pattern)
- Auto-sync when connectivity available
- Bilingual UI (Filipino/English — Taglish)

---

## 🤖 AI Integration

EdukasyonAI uses **NVIDIA Nemotron Nano** for:

| Feature | Description |
|---------|-------------|
| Question Generation | Auto-generate practice problems from lesson content |
| AI Tutor Chat | Bilingual Taglish tutoring (age-appropriate) |
| Adaptive Learning | Adjust difficulty based on student mastery |

**On-Device Inference:**
For offline use, the quantized Nemotron model (4-bit INT8) can be deployed via:
- ONNX Runtime (cross-platform)
- llama.cpp (Android/iOS optimized)

**Safety Guardrails:**
All AI-generated content is filtered for age-appropriate language before being shown to students. AI questions require teacher approval before activation.

---

## 🔐 Security & Compliance

- **JWT Authentication** with refresh tokens
- **Role-Based Access Control**: Student / Teacher / Parent / SchoolAdmin
- **Data Privacy Act (RA 10173)**: Minimum data collection, audit logging
- **COPPA Compliance**: Under-13 users flagged for parental consent
- **Multi-tenancy**: School-level data isolation
- **Input Validation**: AI content safety guardrails

---

## 🌐 Localization

| Language | Status |
|----------|--------|
| Filipino (Tagalog) | ✅ Primary |
| English | ✅ Primary |
| Cebuano | 🔜 Planned |
| Ilocano | 🔜 Planned |

All lesson content and AI responses are bilingual (Filipino + English).

---

## 📊 Architecture

```
Mobile App (MAUI)          Admin Web (Blazor)
       │                          │
       └──────────┬───────────────┘
                  ▼
         REST API (ASP.NET Core)
              │         │
    ┌─────────┘         └──────────┐
    ▼                              ▼
Application Layer          AI Layer (Nemotron)
    │                              │
Domain Layer                Infrastructure
    │                        │         │
    └────────────────────────┤         │
                        SQLite      PostgreSQL
                       (Offline)    (Cloud Sync)
```

---

## 🧪 Testing

Tests use **xUnit** + **Moq**:

```
test/
├── Domain.Tests      — AdaptiveLearningService, entity logic
└── Application.Tests — StudentAppService with mocked repositories
```

---

## 📄 License

MIT © EdukasyonAI Team

*Para sa mga mag-aaral ng Pilipinas. (For the students of the Philippines.)*
