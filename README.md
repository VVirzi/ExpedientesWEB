# Expedientes Web

A web-based migration of a desktop (WinForms) application used internally to process and export pharmaceutical invoice data for multiple healthcare clients in Argentina.

## Overview

The application processes invoice files exported from an internal ERP system, enriches them with metadata and ANMAT traceability data, allows manual review and inline editing, and exports the result in client-specific formats (PDF with QR codes, fixed-width TXT files).

## Tech Stack

**Backend**
- ASP.NET Core Web API (.NET 8)
- Clean Architecture (Api / Application / Domain / Infrastructure)
- HtmlAgilityPack — HTML invoice parsing
- iText — PDF generation
- ZXing.Net — QR code generation

**Frontend**
- React 18 + Vite
- React Router DOM
- Tailwind CSS v4
- Axios
- canvas-confetti

## Architecture

The solution is structured following Clean Architecture principles:
ExpedientesWEB/
├── Expedientes.Api # Controllers, DTOs, request/response models
├── Expedientes.Application # Services, mergers, exporters, mappers
├── Expedientes.Domain # Entities, interfaces
├── Expedientes.Infrastructure # File importers, PDF/QR generation
└── expedientes-web/ # React frontend

## Features

- **Multi-client support** — each client has specific file requirements and export formats
- **Three-file import pipeline** — invoices, metadata and ANMAT traceability files
- **Automatic data enrichment** — GTIN, Troquel, Lote and traceability data merged automatically
- **Company filter** — ANMAT file filtered by company name via appsettings.json
- **Inline editing** — affiliate number, purchase order and item fields editable before export
- **Warning system** — alerts for missing GTIN, Troquel or unmatched ANMAT records
- **Client-specific export** — PDF/QR, billing TXT and settlements TXT formats
- **Claymorphism UI** — custom design system with lavender palette and smooth animations

## How It Works

1. User selects a client
2. Uploads the required files (1 to 3 depending on the client)
3. Backend processes and merges the data
4. User reviews and edits the result in an interactive table
5. User exports in the client-specific format

## Getting Started

### Backend
```bash
cd ExpedientesWEB
dotnet restore
dotnet run --project Expedientes.Api
```

### Frontend
```bash
cd expedientes-web
npm install
npm run dev
```

The API runs on `https://localhost:7249` and the frontend on `http://localhost:5173`.

## Status

Actively in use in production.