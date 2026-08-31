using Expedientes.Application.Mergin;
using Expedientes.Application.Services;
using Expedientes.Domain.Entities;
using Expedientes.Domain.Importers;
using Expedientes.Domain.Interfaces;
using Expedientes.Infrastructure.Importers;
using Expedientes.Application.Exporters;
using Expedientes.Infrastructure.Pdf;

var builder = WebApplication.CreateBuilder(args);

// Importers
builder.Services.AddScoped<IFileImporter<ImportedInvoice>, HtmlImportedInvoiceImporter>();
builder.Services.AddScoped<IFileImporter<InvoiceMetadata>, HtmlInvoiceMetadataImporter>();
builder.Services.AddScoped<IFileImporter<AnmatData>, AnmatImporter>();

// Application
builder.Services.AddScoped<IInvoiceProcessingService, InvoiceProcessingService>();
builder.Services.AddScoped<IInvoiceMetadataMerger, InvoiceMetadataMerger>();
builder.Services.AddScoped<IAnmatMerger, AnmatMerger>();

// Exporters
builder.Services.AddScoped<IQrPdfExporter, QrPdfExporter>();
builder.Services.AddScoped<IInvoiceExporter, ClientAExporter>();

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("FrontendDev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

