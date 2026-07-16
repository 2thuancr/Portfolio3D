# AI_IMPLEMENTATION_GUIDE.md
# Portfolio 3D – AI Implementation Guide
Version: 1.0

> This document is the single source of truth for AI coding agents (Claude Code, Codex, Cursor, Gemini CLI, etc.).
> The companion document `Portfolio3D_ABP_Project_Blueprint_v1.1.docx` contains business and architectural details.
> This file contains implementation rules, constraints and workflows.

---

# 1. Project Overview

## Project Name
Portfolio 3D

## Goal
Build a professional 3D portfolio website where users enter a virtual workspace and explore:

- About
- Skills
- Projects
- Contact

The system must:

- Feel impressive and modern.
- Be simple to maintain.
- Avoid over-engineering.
- Be fully manageable by one developer.
- Be production-ready.

---

# 2. Primary Objectives

1. Deliver an impressive portfolio experience.
2. Keep architecture simple.
3. Maintain high code quality.
4. Minimize maintenance cost.
5. Support future extension without premature abstraction.

---

# 3. Non-Goals

DO NOT IMPLEMENT:

❌ Microservices

❌ Multi-tenancy

❌ Redis

❌ RabbitMQ

❌ Kafka

❌ Event Sourcing

❌ CQRS

❌ MediatR

❌ Distributed Event Bus

❌ Service Mesh

❌ Custom Framework

❌ Generic Repository Pattern

❌ Generic Base Service

❌ Multiple implementations "for future use"

❌ Additional abstraction without a real use case

---

# 4. Technology Stack

## Backend

- ASP.NET Core
- ABP Framework
- Entity Framework Core
- PostgreSQL

## Frontend

- Angular
- TypeScript
- Three.js
- GSAP

## Testing

- xUnit
- FluentAssertions
- Angular Test Runner
- Playwright

## Deployment

- Docker
- Nginx
- Linux VPS

---

# 5. Architecture Style

ABP Layered Monolith.

## Dependency Direction

HttpApi.Host
↓
HttpApi
↓
Application
↓
Domain
↓
Domain.Shared

EntityFrameworkCore
↓
Domain

Angular
↓
HttpApi.Host

---

# 6. Solution Structure

```text
backend/
├── src/
│   ├── Portfolio3D.Domain.Shared
│   ├── Portfolio3D.Domain
│   ├── Portfolio3D.Application.Contracts
│   ├── Portfolio3D.Application
│   ├── Portfolio3D.EntityFrameworkCore
│   ├── Portfolio3D.HttpApi
│   └── Portfolio3D.HttpApi.Host
│
└── test/
    ├── Domain.Tests
    ├── Application.Tests
    └── EntityFrameworkCore.Tests

angular/
├── core
├── features
├── three
└── shared
```

---

# 7. Architecture Rules

## Domain

Domain:

- contains entities
- contains business rules
- contains invariants

Domain MUST NOT:

- reference Application
- reference EF Core
- reference HttpApi
- reference Angular
- reference Three.js

---

## Application

Application:

- orchestrates use cases
- authorization
- validation
- DTO mapping

Application MUST NOT:

- return entities
- contain rendering code
- contain SQL

---

## Infrastructure

Infrastructure:

- persistence
- configuration
- migrations

---

## Frontend

Frontend:

- UI
- API consumption
- 3D rendering

Frontend MUST NOT:

- duplicate backend business rules.

---

# 8. ABP Conventions

Use ABP conventions whenever possible.

Prefer:

- IRepository<T, Guid>
- UnitOfWork
- ApplicationService
- DTOs
- Permission Definitions
- Validation Attributes

Avoid:

- Custom repositories for simple CRUD.
- Custom UnitOfWork implementation.
- Additional abstractions.

---

# 9. Coding Standards

## Backend

### MUST

- Nullable enabled.
- Async suffix.
- Constructor injection.
- CancellationToken for I/O.
- DTOs only.

### DO NOT

- Service Locator.
- Static mutable state.
- Generic catch(Exception).
- Premature abstractions.

---

## Frontend

### MUST

- Typed models.
- Standalone components.
- Cleanup subscriptions.
- Cleanup listeners.
- Cleanup animation frame.

### DO NOT

- use any
- place business logic inside components
- create giant services

---

# 10. Three.js Rules

Create the following services:

```text
SceneEngineService
RendererService
AssetLoaderService
CameraControllerService
RaycastInteractionService
QualityManagerService
```

## Rules

- Dispose geometry.
- Dispose materials.
- Dispose textures.
- Pause rendering when tab hidden.
- Raycast only on interactive objects.
- Maintain 2D fallback.

---

# 11. Performance Budget

Initial JS:
<= 500 KB

3D assets:
<= 8 MB

Desktop FPS:
>= 50

Mobile FPS:
>= 30

LCP:
<= 2.5 seconds

---

# 12. Domain Model

## Profile

- DisplayName
- Headline
- Bio
- AvatarUrl
- CvUrl

---

## Project

- Id
- Name
- Slug
- Summary
- Description
- ThumbnailUrl
- DemoUrl
- RepositoryUrl
- IsPublished
- IsFeatured
- DisplayOrder

---

## Skill

- Id
- Name
- Category
- Level
- DisplayOrder

---

## ContactMessage

- Id
- FullName
- Email
- Subject
- Message
- Status

---

# 13. Permissions

```text
Portfolio3D

Projects.Create
Projects.Update
Projects.Delete
Projects.Publish

Skills.Create
Skills.Update
Skills.Delete

Profile.Update

ContactMessages.View
ContactMessages.Update
```

---

# 14. Public API

GET /api/app/public-portfolio

GET /api/app/project

GET /api/app/project/by-slug/{slug}

POST /api/app/contact-message

---

# 15. Admin API

GET /api/app/project

POST /api/app/project

PUT /api/app/project/{id}

DELETE /api/app/project/{id}

---

# 16. UI Flow

Loading
↓
Enter
↓
Exterior
↓
Corridor
↓
Main Room
↓
Projects
↓
Contact

---

# 17. Scene Manifest Contract

```json
{
  "interactiveObjects": [],
  "cameraTargets": {}
}
```

---

# 18. Quality Profiles

High

Medium

Low

2D Fallback

---

# 19. Testing Strategy

## Backend

- Domain Tests
- Application Tests
- Integration Tests

## Frontend

- Unit Tests
- E2E Tests

## Performance

- Lighthouse
- Manual FPS tests

---

# 20. Deployment

Docker
Nginx
HTTPS

Environment variables only.

No secrets in source code.

---

# 21. Branch Strategy

main

feature/*

fix/*

---

# 22. Pull Request Checklist

- Build passes.
- Tests pass.
- No lint errors.
- No unrelated files changed.
- Architecture respected.

---

# 23. Development Workflow

For every task:

1. Read existing files.
2. Understand architecture.
3. Implement.
4. Build.
5. Run tests.
6. Report.

---

# 24. Report Format

Always provide:

## Changed Files

## Decisions

## Commands Executed

## Remaining Issues

---

# 25. Task Template

## Goal

## Requirements

## Files

## Acceptance Criteria

## Commands

---

# 26. Current Project Status

Completed:

-

In Progress:

-

Next:

1. Setup ABP solution
2. Configure PostgreSQL
3. Project CRUD
4. Public 2D shell
5. 3D scene
6. Camera path
7. Hotspots
8. Contact
9. Deployment

---

# 27. Guardrails Against Over-Engineering

DO NOT:

- create abstractions for future possibilities
- create interfaces with one implementation unless justified
- create custom repositories for CRUD
- add event bus without real requirements
- add patterns because they are popular
- optimize before measuring

Prefer:

- convention over configuration
- composition over abstraction
- simplicity over cleverness
- vertical slices
- measurable decisions

---

# 28. Definition of Done

A task is DONE only if:

- code builds
- tests pass
- no memory leaks
- architecture respected
- no unrelated changes
- documentation updated when necessary

---

# 29. AI Operating Principles

You are a senior software engineer.

You must:

- prefer simple solutions
- explain tradeoffs
- ask when requirements are ambiguous
- avoid assumptions
- preserve architecture
- preserve maintainability
- preserve readability

Never rewrite large parts of the project without explicit instructions.
