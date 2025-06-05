# Ton Wallet

This repository contains a proof-of-concept wallet for the TON blockchain. The code base is split into a .NET back-end and a Vue front-end.

## Repository layout

```
src/
├─ dotnet/           # .NET solution with API and Telegram bot
└─ tg-vite-app/      # Front-end application on Vite/Vue
```

### Back-end (`src/dotnet`)
* **TonWalletApi** – REST API configuring CORS, JWT authentication and PostgreSQL access. Swagger is enabled by default.
* **TonWalletBot** – Telegram bot with webhook configuration.
* **TonWallet.Domain** / **TonWallet.Infrastructure** – domain entities and EF Core context with migrations.
* `docker-compose.yml` – draft file for running services via Docker.

The solution file `TonWalletTest.sln` ties all projects together. You can build and run the API from this folder using `dotnet build` and `dotnet run`, or adjust the provided `docker-compose` setup.

### Front-end (`src/tg-vite-app`)
The front-end is a regular Vue 3 project created with Vite. Key files:

* `src/components` – Vue components such as `HomeComponent.vue` and `JettonChart.vue`.
* `src/services` – wrapper around Axios for API requests and token refresh logic.
* `stores` – Pinia store for client-side state.

See `src/tg-vite-app/README.md` for all available npm commands (`npm run dev`, `npm run build`, tests, linting, etc.).

## Running the project

1. **Install the front-end dependencies**
   ```bash
   cd src/tg-vite-app
   npm install
   npm run dev
   ```
   This starts the Vite dev server.

2. **Run the back-end**
   ```bash
   cd src/dotnet
   dotnet build
   dotnet run --project TonWalletApi
   ```
   Alternatively use `docker-compose up` once the compose file is finalised.

Environment variables should be used for secrets such as database passwords, JWT keys and Telegram tokens. The provided `appsettings.json` files contain sample values for local development only.

## What to explore next

* `TonWalletApi/Controllers` and `TonWalletApi/Services` – API endpoints and services for working with tonapi.io and authentication.
* `src/tg-vite-app/src` – Vue components, Pinia store and service classes (`AuthService.ts`, `axiosConfig.ts`).
* Improve the Docker configuration and consider adding more tests for both API and front-end.

