import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"
import { exportInvoices } from "../api/invoiceApi"

export default function InvoiceReviewPage() {
  const { invoiceResult, setInvoiceResult, selectedClient } = useInvoice()
  const navigate = useNavigate()
  const [selectedInvoice, setSelectedInvoice] = useState(null)
  const [showExportOptions, setShowExportOptions] = useState(false)

  if (!invoiceResult) {
    navigate("/")
    return null
  }

  function handleItemEdit(invoiceNumber, itemIndex, field, value) {
    setInvoiceResult(prev => ({
      ...prev,
      invoices: prev.invoices.map(invoice => {
        if (invoice.invoiceNumber !== invoiceNumber) return invoice
        const updatedItems = invoice.items.map((item, i) => {
          if (i !== itemIndex) return item
          return { ...item, [field]: value }
        })
        return { ...invoice, items: updatedItems }
      })
    }))

    if (selectedInvoice?.invoiceNumber === invoiceNumber) {
      setSelectedInvoice(prev => ({
        ...prev,
        items: prev.items.map((item, i) => {
          if (i !== itemIndex) return item
          return { ...item, [field]: value }
        })
      }))
    }
  }

  function handleInvoiceEdit(invoiceNumber, field, value) {
    setInvoiceResult(prev => ({
      ...prev,
      invoices: prev.invoices.map(invoice => {
        if (invoice.invoiceNumber !== invoiceNumber) return invoice
        return { ...invoice, [field]: value }
      })
    }))

    if (selectedInvoice?.invoiceNumber === invoiceNumber) {
      setSelectedInvoice(prev => ({ ...prev, [field]: value }))
    }
  }

  function handleRowClick(invoice) {
    setSelectedInvoice(
      selectedInvoice?.invoiceNumber === invoice.invoiceNumber ? null : invoice
    )
  }

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-7xl mx-auto">

        <div className="flex items-center justify-between mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-800">Revisión de Facturas</h1>
            <p className="text-sm text-gray-500">Cliente: {selectedClient?.name}</p>
          </div>
          <div className="flex gap-3">
            <button
              onClick={() => navigate("/upload")}
              className="px-4 py-2 rounded-xl border border-gray-300 text-gray-600 hover:bg-gray-50 transition text-sm"
            >
              Volver
            </button>

            {selectedClient?.id === "ClientB" ? (
              <div className="relative">
                <button
                  onClick={() => setShowExportOptions(!showExportOptions)}
                  className="px-4 py-2 rounded-xl bg-blue-600 text-white font-medium hover:bg-blue-700 transition text-sm"
                >
                  Exportar ▾
                </button>
                {showExportOptions && (
                  <div className="absolute right-0 mt-2 bg-white border border-gray-200 rounded-xl shadow-lg z-10 overflow-hidden">
                    <button
                      onClick={() => {
                        exportInvoices("ClientB", "billing", invoiceResult)
                        setShowExportOptions(false)
                      }}
                      className="block w-full text-left px-5 py-3 text-sm text-gray-700 hover:bg-blue-50 transition"
                    >
                      Facturación
                    </button>
                    <button
                      onClick={() => {
                        exportInvoices("ClientB", "settlements", invoiceResult)
                        setShowExportOptions(false)
                      }}
                      className="block w-full text-left px-5 py-3 text-sm text-gray-700 hover:bg-blue-50 transition border-t border-gray-100"
                    >
                      Liquidaciones
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <button
                onClick={() => exportInvoices(
                  selectedClient?.id,
                  selectedClient?.id === "ClientA" ? "pdf" : "txt",
                  invoiceResult
                )}
                className="px-4 py-2 rounded-xl bg-blue-600 text-white font-medium hover:bg-blue-700 transition text-sm"
              >
                Exportar
              </button>
            )}
          </div>
        </div>

        {invoiceResult.warnings.length > 0 && (
          <div className="mb-6 bg-yellow-50 border border-yellow-200 rounded-xl p-4">
            <h3 className="text-sm font-semibold text-yellow-800 mb-2">⚠️ Advertencias</h3>
            {invoiceResult.warnings.map((w, i) => (
              <p key={i} className="text-sm text-yellow-700">{w.invoiceNumber} — {w.message}</p>
            ))}
          </div>
        )}

        <div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden mb-6">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                {["Tipo", "Número", "Fecha", "Remito", "Afiliado", "Nº Afiliado", "O. Compra", "Total", "CAE"].map(h => (
                  <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
                    {h}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {invoiceResult.invoices.map(invoice => (
                <tr
                  key={invoice.invoiceNumber}
                  onClick={() => handleRowClick(invoice)}
                  className={`cursor-pointer transition-colors hover:bg-blue-50 ${
                    selectedInvoice?.invoiceNumber === invoice.invoiceNumber
                      ? "bg-blue-50 border-l-4 border-l-blue-500"
                      : ""
                  }`}
                >
                  <td className="px-4 py-3 font-medium text-gray-700">{invoice.invoiceType}</td>
                  <td className="px-4 py-3 text-gray-600">{invoice.invoiceNumber}</td>
                  <td className="px-4 py-3 text-gray-600">
                    {new Date(invoice.date).toLocaleDateString("es-AR")}
                  </td>
                  <td className="px-4 py-3 text-gray-600">{invoice.remitoNumber}</td>
                  <td className="px-4 py-3" onClick={e => e.stopPropagation()}>
                    <EditableCell
                      value={invoice.affiliateName}
                      onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "affiliateName", v)}
                    />
                  </td>
                  <td className="px-4 py-3" onClick={e => e.stopPropagation()}>
                    <EditableCell
                      value={invoice.affiliateNumber}
                      onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "affiliateNumber", v)}
                    />
                  </td>
                  <td className="px-4 py-3" onClick={e => e.stopPropagation()}>
                    <EditableCell
                      value={invoice.purchaseOrder}
                      onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "purchaseOrder", v)}
                    />
                  </td>
                  <td className="px-4 py-3 text-gray-600">
                    ${(invoice.totalAmount / 100).toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                  </td>
                  <td className="px-4 py-3 text-gray-500 font-mono text-xs">{invoice.cae}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {selectedInvoice && (
          <div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100">
              <h3 className="font-semibold text-gray-800">
                Ítems — {selectedInvoice.invoiceNumber}
              </h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-gray-50 border-b border-gray-200">
                  <tr>
                    {["Artículo", "GTIN", "Troquel", "Lote", "Cantidad", "Precio Unit.", "Trazabilidades"].map(h => (
                      <th key={h} className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
                        {h}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {selectedInvoice.items.map((item, i) => (
                    <tr key={i} className="hover:bg-gray-50">
                      <td className="px-4 py-2">
                        <EditableCell value={item.article} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "article", v)} />
                      </td>
                      <td className="px-4 py-2">
                        <EditableCell value={item.gtin} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "gtin", v)} />
                      </td>
                      <td className="px-4 py-2">
                        <EditableCell value={item.troquel} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "troquel", v)} />
                      </td>
                      <td className="px-4 py-2">
                        <EditableCell value={item.lote} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "lote", v)} />
                      </td>
                      <td className="px-4 py-2">
                        <EditableCell value={item.quantity} type="number" onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "quantity", parseInt(v))} />
                      </td>
                      <td className="px-4 py-2">
                        <EditableCell value={item.unitPrice} type="number" onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "unitPrice", parseFloat(v))} />
                      </td>
                      <td className="px-4 py-3 text-center">
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                          item.traceabilities.length > 0
                            ? "bg-green-100 text-green-700"
                            : "bg-gray-100 text-gray-500"
                        }`}>
                          {item.traceabilities.length}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

      </div>
    </div>
  )
}

function EditableCell({ value, onChange, type = "text" }) {
  return (
    <input
      type={type}
      value={value ?? ""}
      onChange={e => onChange(e.target.value)}
      className="w-full bg-transparent border border-transparent rounded-lg px-2 py-1 hover:border-gray-300 focus:border-blue-400 focus:outline-none focus:bg-white transition text-gray-700 text-sm"
    />
  )
}