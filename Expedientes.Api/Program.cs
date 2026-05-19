using Expedientes.Application.Mergin;
using Expedientes.Application.Services;
using Expedientes.Domain.Entities;
using Expedientes.Domain.Importers;
using Expedientes.Domain.Interfaces;
using Expedientes.Infrastructure.Importers;

var builder = WebApplication.CreateBuilder(args);

// Importers
builder.Services.AddScoped<IFileImporter<ImportedInvoice>, HtmlImportedInvoiceImporter>();
builder.Services.AddScoped<IFileImporter<InvoiceMetadata>, HtmlInvoiceMetadataImporter>();
builder.Services.AddScoped<IFileImporter<AnmatData>, AnmatImporter>();

// Application
builder.Services.AddScoped<IInvoiceProcessingService, InvoiceProcessingService>();
builder.Services.AddScoped<IInvoiceMetadataMerger, InvoiceMetadataMerger>();
builder.Services.AddScoped<IAnmatMerger, AnmatMerger>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

