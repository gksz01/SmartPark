# SmartPark — Fase 1 (núcleo de reservas)

Plataforma de estacionamento inteligente (arc42 TDE Entrega 1). Esta fase cobre RF01–RF10 e RF24:
cadastro e autenticação por perfil, veículos, localização e disponibilidade de estacionamentos,
reserva de **capacidade** (não de vaga) sem overbooking, cancelamento com estorno, pagamento por
gateway simulado e administração de estacionamentos, vagas e usuários operacionais.

Ficam para a fase 2: sensores IoT (RF11), entrada/saída por placa e cancelas (RF12–RF19),
notificações (RF20), operação assistida e liberação manual (RF21–RF23) e painel do administrador (RF25).

## Como rodar

Pré-requisitos: .NET SDK 10, Docker em execução.

```powershell
dotnet run --project SmartPark.AppHost --launch-profile https
```

O Aspire sobe PostgreSQL (+ pgAdmin) → `migrations` (aplica migrations e seed, depois encerra) → `api` → `web`.
O link do dashboard aparece no console.

| Item | Valor (desenvolvimento) |
|---|---|
| Web | https://localhost:7114 |
| API + Scalar | https://localhost:7183/scalar |
| Administrador | `admin@smartpark.local` / senha do parâmetro `admin-password` |
| Motorista demo | `motorista@smartpark.local` / mesma senha |

Os segredos ficam nos user-secrets do AppHost (`Parameters:jwt-key`, `Parameters:admin-password`).
Se faltarem, o dashboard pede os valores.

**SQL Server local:** `$env:SMARTPARK_DB_PROVIDER = "SqlServer"` antes de subir o AppHost. Em publish o
AppHost usa sempre SQL Server, recebendo a connection string `smartparkdb`.

**Gateway simulado:** aprova tudo, exceto cartões terminados em `0000`. `GatewayPagamento:SimularIndisponibilidade=true`
na API simula o gateway fora do ar.

## Estrutura

Sem pasta `src`: cada projeto fica na raiz.

| Projeto | Papel |
|---|---|
| `SmartPark.Domain` | Entidades, value objects, `Result`/`Error`. Sem dependências. |
| `SmartPark.Contracts` | DTOs HTTP compartilhados entre API e Web. |
| `SmartPark.Application` | `Features/<Módulo>/<CasoDeUso>/` com Command/Query + Handler + Validator; behaviors do Mediator; portas. |
| `SmartPark.Infrastructure` | EF Core (`SmartParkDbContext`), Identity, JWT, gateway simulado, escolha do provider. |
| `SmartPark.Migrations.PostgreSql` / `.SqlServer` | Migrations de cada provider. |
| `SmartPark.MigrationService` | Worker que migra e faz o seed. |
| `SmartPark.Api` | Host: `Features/<Módulo>/<CasoDeUso>Endpoint.cs`, gerador do Mediator, autenticação. |
| `SmartPark.Web` | Blazor (Interactive Server) com Blazor Blueprint, organizado por `Features/`. |
| `SmartPark.ArchitectureTests` | Regras de camadas, convenções de vertical slice e encapsulamento do domínio. |
| `SmartPark.AppHost` / `.ServiceDefaults` | Orquestração Aspire. |

## Comandos úteis

```powershell
dotnet build SmartPark.slnx
dotnet test --project SmartPark.ArchitectureTests

# Nova migration: sempre nos dois providers
dotnet ef migrations add <Nome> --project SmartPark.Migrations.PostgreSql --output-dir Migrations
dotnet ef migrations add <Nome> --project SmartPark.Migrations.SqlServer --output-dir Migrations
```

## Overbooking (RF08)

A disponibilidade de um período é `capacidade reservável − pico de reservas simultâneas`. A confirmação
troca o token de concorrência (`Versao`) do estacionamento na mesma gravação da reserva; se outra reserva
for confirmada no meio, a gravação falha e a capacidade é recalculada (até 3 tentativas). Duas reservas
simultâneas pela última vaga resultam em uma 201 e uma 409.
