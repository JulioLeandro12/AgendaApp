# AgendaApp

Gerenciamento de contatos (agenda) com API em ASP.NET Core 8, frontend em Vue 3 + PrimeVue e orquestração via Docker Compose. Inclui validação avançada, mensagens de erro padronizadas (ProblemDetails), suporte a múltiplos países para formatação de telefone e documentação Swagger.

## Sumário
- Visão Geral
- Principais Funcionalidades
- Stack Tecnológica
- Estrutura de Pastas
- Arquitetura (Camadas & Fluxo)
- Configuração e Execução
	- Rodando com Docker
	- Rodando local (sem Docker)
- Variáveis de Ambiente
- Migrações & Seed
- Validação & Erros (FluentValidation + ProblemDetails)
- Telefone & País (Formatação / Armazenamento)
- Endpoints da API (Resumo)
- Troubleshooting (Problemas Comuns)
- Próximos Passos / Extensões

## Visão Geral
O projeto oferece uma API REST para CRUD de contatos (nome, email, telefone) e uma interface web para criação, listagem, edição e exclusão. O frontend realiza chamadas à API, exibe mensagens de sucesso/erro e formata telefones automaticamente conforme o país selecionado.

## Principais Funcionalidades
- CRUD completo de contatos.
- Validação de campos (nome, email, telefone) com regras claras.
- Tratamento de duplicidade (nome e email) retornando HTTP 409.
- Mensagens de erro padronizadas (ProblemDetails) consumidas pelo frontend.
- Formatação de números internacionais com seleção de país (Brasil, EUA, Portugal) e armazenamento em formato E.164 simplificado.
- Swagger UI para documentação e teste rápido da API.
- Orquestração com Docker Compose (Postgres, Backend, Frontend).

## Stack Tecnológica
- Backend: .NET 8, ASP.NET Core Web API, Entity Framework Core (PostgreSQL), FluentValidation, Swagger (Swashbuckle).
- Banco: PostgreSQL 15.
- Frontend: Vue 3, Vite, PrimeVue, Axios, Nginx (produção).
- Containerização: Docker Compose.

## Estrutura de Pastas (Resumo)
```
backend/
	Controllers/        // Endpoints HTTP
	Services/           // Regra de negócio (ContactService)
	Repositories/       // Acesso a dados (EF Core)
	Data/               // DbContext e Seed
	Models/             // Entidades
	DTOs/               // Data Transfer Objects
	Validators/         // FluentValidation
	Migrations/         // Migrações EF
frontend/
	src/
		api/              // Axios instance & chamadas
		components/       // Componentes Vue (ContactList)
		views/            // Páginas
		router/           // Rotas Vue
		assets/           // Estáticos
	nginx.conf          // Proxy / Config produção
docker-compose.yml    // Orquestração dos serviços
README.md
```

## Arquitetura (Camadas & Fluxo)
1. Controller recebe requisição → delega ao Service.
2. Service aplica regras (duplicidade, mapeamento DTO ↔ modelo) → chama Repository.
3. Repository usa EF Core (AppDbContext) para persistência no Postgres.
4. Respostas de erro (validação ou regras) geram ProblemDetails estruturado.
5. Frontend consome API via Axios; interceptor traduz erros em mensagens exibidas via Toast.

## Configuração e Execução

### Pré‑requisitos
- .NET 8 SDK
- Node.js 18+ e npm
- Docker Desktop (se usar containers)

### Rodando com Docker (recomendado)
```powershell
docker-compose up -d --build
```
Serviços expostos:
- Backend: http://localhost:5000 (Swagger em /swagger)
- Frontend: http://localhost:5174
- Postgres: porta 5432 (opcional para acesso externo)

Parar containers:
```powershell
docker-compose down
```

### Rodando local (sem Docker)
1. Banco Postgres local ou remoto disponível.
2. Ajustar connection string em `backend/appsettings.Development.json` ou variável de ambiente `ConnectionStrings__DefaultConnection`.
3. Backend:
	 ```powershell
	 cd backend
	 dotnet restore
	 dotnet ef database update   # aplica migrações
	 dotnet run                  # inicia API em http://localhost:5000
	 ```
4. Frontend (usar variável apontando para API local):
	 ```powershell
	 cd frontend
	 npm install
	 setx VITE_API_URL "http://localhost:5000/api"  # PowerShell: define persistent env (reabrir terminal)
	 npm run dev                                      # http://localhost:5173
	 ```
	 Ou usar `.env.development` com: `VITE_API_URL=http://localhost:5000/api`.

## Variáveis de Ambiente Importantes
- `ConnectionStrings__DefaultConnection` (backend) – string de conexão Postgres.
- `ASPNETCORE_ENVIRONMENT` – controla uso de Swagger/dev middleware.
- `VITE_API_URL` (frontend) – base URL para API quando não usando proxy Nginx.

## Migrações & Seed
- Migrações geradas e aplicadas automaticamente no startup quando em Docker.
- Classe `SeedData.Initialize` popula contatos de exemplo se tabela estiver vazia.
Gerar nova migração:
```powershell
cd backend
dotnet ef migrations add NomeDaMigracao
dotnet ef database update
```

## Validação & Erros
- FluentValidation em `Validators/ContactDtoValidator.cs`.
- Regras: nome, email e telefone obrigatórios; formato de email válido; telefone internacional (E.164 simplificado).
- Duplicidade (nome/email) gera HTTP 409 com `ProblemDetails.detail` esclarecendo o conflito.
- Erros de validação geram HTTP 400 com `errors` por campo.
- Frontend: interceptor Axios extrai `errors` (array) ou `detail` e mostra cada mensagem em Toast.

## Telefone & País
- Armazenado como string iniciando com `+` seguido de DDI + número nacional (E.164 simples).
- Frontend permite seleção de país (BR +55, US +1, PT +351) ao criar/editar.
- Formatação de exibição é inferida por DDI no grid (`formatPhoneTable`), não muda ao trocar país no formulário.
- Placeholder dinâmico ajuda o usuário a digitar corretamente.

## Endpoints da API (Resumo)
| Método | Rota                | Descrição                    |
|--------|---------------------|------------------------------|
| GET    | `/api/contacts`     | Lista todos os contatos      |
| GET    | `/api/contacts/{id}`| Obtém contato por ID         |
| POST   | `/api/contacts`     | Cria novo contato            |
| PUT    | `/api/contacts/{id}`| Atualiza contato existente   |
| DELETE | `/api/contacts/{id}`| Remove contato               |

Exemplo de criação (PowerShell):
```powershell
curl -Method POST -Uri http://localhost:5000/api/contacts -ContentType application/json -Body '{"name":"João","email":"joao@example.com","phone":"+5581986941088"}'
```

## Troubleshooting
| Sintoma | Causa Provável | Solução |
|---------|----------------|---------|
| 404 no frontend `/api/contacts` | Proxy Nginx ausente ou `VITE_API_URL` incorreto | Definir `VITE_API_URL` ou reconstruir container frontend |
| Erro JSON id:null | Payload enviando `id:null` na criação | Remover `id` antes de POST (já implementado) |
| 409 Conflito | Nome/Email duplicado | Alterar dados ou consultar lista antes de criar |
| Telefone inválido | Formato não segue regex E.164 | Usar `+<ddi><número>` sem caracteres extras (espaços/hífens opcionais) |
| Seed não executa | Tabela já contém registros | Limpar tabela manualmente se quiser re‑seed |

## Próximos Passos / Extensões (Sugestões)
- Paginação server-side e filtros (nome/email) na API.
- Testes automatizados (unitários e integração) para Service/Controller.
- Autenticação/JWT para proteger endpoints.
- Upload de avatar e campos adicionais (empresa, endereço). 
- Internacionalização completa (i18n) no frontend.

## Licença
Projeto de exemplo interno. Adicione uma licença (MIT, Apache 2.0, etc.) se necessário.

---
Qualquer dúvida ou sugestão adicional, fique à vontade para abrir uma issue ou evoluir o projeto. Bom uso!
