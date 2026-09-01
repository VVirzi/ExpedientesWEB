import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"
import { exportInvoices } from "../api/invoiceApi"
import confetti from "canvas-confetti"

export default function InvoiceReviewPage() {
  const { invoiceResult, setInvoiceResult, selectedClient } = useInvoice()
  const navigate = useNavigate()
  const [selectedInvoice, setSelectedInvoice] = useState(null)
  const [showExportOptions, setShowExportOptions] = useState(false)
  const [exporting, setExporting] = useState(false)
  const [exportError, setExportError] = useState(null)

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

  async function handleExport(clientId, exportType) {
    setExporting(true)
    setExportError(null)
    try {
      await exportInvoices(clientId, exportType, invoiceResult)
      confetti({
        particleCount: 120,
        spread: 80,
        origin: { x: 0.5, y: 1.1 },
        angle: 90,
        colors: ["#6B4FD8", "#8B6FE8", "#F0EDF9", "#d9d4f1"]
      })
    } catch (err) {
      setExportError("Error al exportar. Verificá que los datos sean correctos.")
    } finally {
      setExporting(false)
    }
  }

  const headerStyle = {
    fontSize: "0.7rem",
    fontWeight: 600,
    color: "var(--muted)",
    textTransform: "uppercase",
    letterSpacing: "0.08em",
    padding: "0.75rem 1rem",
    textAlign: "center",
    borderBottom: "1px solid var(--border)"
  }

  const cellStyle = {
    padding: "0.75rem 1rem",
    fontSize: "0.85rem",
    color: "var(--text)",
    borderBottom: "1px solid rgba(212, 206, 240, 0.4)",
    textAlign: "center"
  }

  return (
    <div style={{ minHeight: "100vh", background: "var(--bg)", padding: "2rem" }}>
      <div style={{ maxWidth: "1200px", margin: "0 auto" }}>

        {/* Header */}
        <div style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          marginBottom: "2rem"
        }}>
          <div>
            <h1 style={{
              fontSize: "1.5rem",
              fontWeight: 700,
              color: "var(--text)",
              letterSpacing: "-0.02em",
              marginBottom: "0.2rem",
              textShadow: "2px 2px 6px rgba(160,150,200,0.5), -1px -1px 3px rgba(255,255,255,0.8)"
            }}>
              Revisión de Facturas
            </h1>
            <p style={{ fontSize: "0.8rem", color: "var(--muted)" }}>
              Cliente: {selectedClient?.name}
            </p>
          </div>

          <div style={{ display: "flex", gap: "0.75rem" }}>
            <button onClick={() => navigate("/upload")} className="btn-secondary">
              Volver
            </button>

            {selectedClient?.id === "ClientB" ? (
              <div style={{ position: "relative" }}>
                <button
                  onClick={() => setShowExportOptions(!showExportOptions)}
                  disabled={exporting}
                  className="btn-primary"
                >
                  {exporting ? "Exportando..." : "Exportar ▾"}
                </button>
                {showExportOptions && (
                  <div style={{
                    position: "absolute",
                    right: 0,
                    marginTop: "0.5rem",
                    background: "#E8E4F5",
                    border: "none",
                    borderRadius: "0.75rem",
                    overflow: "hidden",
                    zIndex: 10,
                    boxShadow: "6px 6px 14px rgba(180,170,220,0.7), -6px -6px 14px rgba(220,216,240,0.7), inset 0 2px 4px rgba(255,255,255,0.6), inset 0 -2px 4px rgba(160,150,200,0.3)",
                    minWidth: "160px"
                  }}>
                    <button
                      onClick={() => { handleExport("ClientB", "billing"); setShowExportOptions(false) }}
                      style={{
                        display: "block", width: "100%", textAlign: "left",
                        padding: "0.75rem 1.25rem", background: "transparent",
                        border: "none", color: "var(--text)", cursor: "pointer",
                        fontSize: "0.85rem", fontWeight: 500, transition: "background 0.15s"
                      }}
                      onMouseEnter={e => e.currentTarget.style.background = "rgba(107,79,216,0.08)"}
                      onMouseLeave={e => e.currentTarget.style.background = "transparent"}
                    >
                      Facturación
                    </button>
                    <button
                      onClick={() => { handleExport("ClientB", "settlements"); setShowExportOptions(false) }}
                      style={{
                        display: "block", width: "100%", textAlign: "left",
                        padding: "0.75rem 1.25rem", background: "transparent",
                        border: "none", borderTop: "1px solid rgba(212,206,240,0.5)",
                        color: "var(--text)", cursor: "pointer",
                        fontSize: "0.85rem", fontWeight: 500, transition: "background 0.15s"
                      }}
                      onMouseEnter={e => e.currentTarget.style.background = "rgba(107,79,216,0.08)"}
                      onMouseLeave={e => e.currentTarget.style.background = "transparent"}
                    >
                      Liquidaciones
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <button
                onClick={() => handleExport(
                  selectedClient?.id,
                  selectedClient?.id === "ClientA" ? "pdf" : "txt"
                )}
                disabled={exporting}
                className="btn-primary"
              >
                {exporting ? "Exportando..." : "Exportar"}
              </button>
            )}
          </div>
        </div>

        {/* Error exportación */}
        {exportError && (
          <div style={{
            marginBottom: "1.5rem", padding: "0.75rem 1rem",
            background: "rgba(255,80,80,0.1)", border: "1px solid rgba(255,80,80,0.3)",
            borderRadius: "0.75rem", fontSize: "0.85rem", color: "#cc3333"
          }}>
            {exportError}
          </div>
        )}

        {/* Warnings */}
        {invoiceResult.warnings.length > 0 && (
          <div style={{
            marginBottom: "1.5rem", padding: "1rem",
            background: "rgba(180,140,0,0.08)", border: "1px solid rgba(180,140,0,0.25)",
            borderRadius: "0.75rem"
          }}>
            <p style={{ fontSize: "0.8rem", fontWeight: 600, color: "#8a6800", marginBottom: "0.5rem" }}>
              ⚠️ Advertencias
            </p>
            {invoiceResult.warnings.map((w, i) => (
              <p key={i} style={{ fontSize: "0.8rem", color: "#8a6800", opacity: 0.8 }}>
                {w.invoiceNumber} — {w.message}
              </p>
            ))}
          </div>
        )}

        {/* Tabla facturas */}
        <div className="clay" style={{ borderRadius: "1rem", overflow: "hidden", marginBottom: "1.5rem" }}>
          <table style={{ width: "100%", borderCollapse: "collapse" }}>
            <colgroup>
              <col style={{ width: "5%" }} />
              <col style={{ width: "13%" }} />
              <col style={{ width: "7%" }} />
              <col style={{ width: "10%" }} />
              <col style={{ width: "20%" }} />
              <col style={{ width: "12%" }} />
              <col style={{ width: "12%" }} />
              <col style={{ width: "10%" }} />
              <col style={{ width: "18%" }} />
            </colgroup>
            <thead>
              <tr style={{ background: "rgba(107,79,216,0.04)" }}>
                {["Tipo", "Número", "Fecha", "Remito", "Afiliado", "Nº Afiliado", "O. Compra", "Total", "CAE"].map(h => (
                  <th key={h} style={headerStyle}>{h}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {invoiceResult.invoices.map(invoice => (
                <tr
                  key={invoice.invoiceNumber}
                  onClick={() => handleRowClick(invoice)}
                  style={{
                    cursor: "pointer",
                    background: selectedInvoice?.invoiceNumber === invoice.invoiceNumber
                      ? "rgba(107,79,216,0.06)" : "transparent",
                    borderLeft: selectedInvoice?.invoiceNumber === invoice.invoiceNumber
                      ? "3px solid var(--accent)" : "3px solid transparent",
                    transition: "background 0.15s"
                  }}
                  onMouseEnter={e => {
                    if (selectedInvoice?.invoiceNumber !== invoice.invoiceNumber)
                      e.currentTarget.style.background = "rgba(107,79,216,0.03)"
                  }}
                  onMouseLeave={e => {
                    if (selectedInvoice?.invoiceNumber !== invoice.invoiceNumber)
                      e.currentTarget.style.background = "transparent"
                  }}
                >
                  <td style={{ ...cellStyle, fontWeight: 500 }}>{invoice.invoiceType}</td>
                  <td style={cellStyle}>{invoice.invoiceNumber}</td>
                  <td style={{ ...cellStyle, color: "var(--muted)" }}>
                    {new Date(invoice.date).toLocaleDateString("es-AR")}
                  </td>
                  <td style={{ ...cellStyle, color: "var(--muted)", fontFamily: "monospace", fontSize: "0.78rem" }}>
                    {invoice.remitoNumber}
                  </td>
                  <td style={cellStyle} onClick={e => e.stopPropagation()}>
                    <EditableCell value={invoice.affiliateName} onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "affiliateName", v)} />
                  </td>
                  <td style={cellStyle} onClick={e => e.stopPropagation()}>
                    <EditableCell value={invoice.affiliateNumber} onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "affiliateNumber", v)} />
                  </td>
                  <td style={cellStyle} onClick={e => e.stopPropagation()}>
                    <EditableCell value={invoice.purchaseOrder} onChange={v => handleInvoiceEdit(invoice.invoiceNumber, "purchaseOrder", v)} />
                  </td>
                  <td style={{ ...cellStyle, color: "var(--accent)", fontWeight: 600 }}>
                    ${(invoice.totalAmount / 100).toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                  </td>
                  <td style={{ ...cellStyle, color: "var(--muted)", fontFamily: "monospace", fontSize: "0.75rem" }}>
                    {invoice.cae}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* Tabla ítems */}
        {selectedInvoice && (
          <div className="clay" style={{ borderRadius: "1rem", overflow: "hidden" }}>
            <div style={{
              padding: "1rem 1.25rem",
              borderBottom: "1px solid var(--border)",
              display: "flex", alignItems: "center", gap: "0.5rem"
            }}>
              <span style={{ color: "var(--accent)", fontSize: "0.8rem" }}>◈</span>
              <span style={{ fontSize: "0.9rem", fontWeight: 600, color: "var(--text)" }}>
                Ítems — {selectedInvoice.invoiceNumber}
              </span>
            </div>
            <div style={{ overflowX: "auto" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <colgroup>
                  <col style={{ width: "22%" }} />
                  <col style={{ width: "12%" }} />
                  <col style={{ width: "9%" }} />
                  <col style={{ width: "10%" }} />
                  <col style={{ width: "8%" }} />
                  <col style={{ width: "7%" }} />
                  <col style={{ width: "12%" }} />
                  <col style={{ width: "8%" }} />
                </colgroup>
                <thead>
                  <tr style={{ background: "rgba(107,79,216,0.04)" }}>
                    {["Artículo", "GTIN", "Troquel", "Lote", "Fecha V", "Cantidad", "Precio Unit.", "Trazabilidades"].map(h => (
                      <th key={h} style={headerStyle}>{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {selectedInvoice.items.map((item, i) => (
                    <tr
                      key={i}
                      style={{ transition: "background 0.15s" }}
                      onMouseEnter={e => e.currentTarget.style.background = "rgba(107,79,216,0.03)"}
                      onMouseLeave={e => e.currentTarget.style.background = "transparent"}
                    >
                      <td style={cellStyle}>
                        <EditableCell value={item.article} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "article", v)} />
                      </td>
                      <td style={cellStyle}>
                        <EditableCell value={item.gtin} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "gtin", v)} />
                      </td>
                      <td style={cellStyle}>
                        <EditableCell value={item.troquel} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "troquel", v)} />
                      </td>
                      <td style={cellStyle}>
                        <EditableCell value={item.lote} onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "lote", v)} />
                      </td>
                      <td style={{ ...cellStyle, color: "var(--muted)", fontSize: "0.8rem" }}>
                        {item.expirationDate
                          ? new Date(item.expirationDate).toLocaleDateString("es-AR")
                          : "—"}
                      </td>
                      <td style={cellStyle}>
                        <EditableCell value={item.quantity} type="number" onChange={v => handleItemEdit(selectedInvoice.invoiceNumber, i, "quantity", parseInt(v))} />
                      </td>
                      <td style={{ ...cellStyle, color: "var(--accent)", fontWeight: 500 }}>
                        ${(item.unitPrice / 100).toLocaleString("es-AR", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                      </td>
                      <td style={{ ...cellStyle, textAlign: "center" }}>
                        <span style={{
                          display: "inline-block",
                          padding: "0.2rem 0.6rem",
                          borderRadius: "99px",
                          fontSize: "0.75rem",
                          fontWeight: 600,
                          background: item.traceabilities.length > 0
                            ? "rgba(107,79,216,0.12)" : "rgba(0,0,0,0.05)",
                          color: item.traceabilities.length > 0
                            ? "var(--accent)" : "var(--muted)"
                        }}>
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
      style={{
        width: "100%",
        background: "transparent",
        border: "1px solid transparent",
        borderRadius: "0.4rem",
        padding: "0.25rem 0.4rem",
        color: "var(--text)",
        fontSize: "0.85rem",
        outline: "none",
        textAlign: "center",
        transition: "border-color 0.15s, background 0.15s"
      }}
      onFocus={e => {
        e.currentTarget.style.borderColor = "var(--accent)"
        e.currentTarget.style.background = "rgba(107,79,216,0.05)"
      }}
      onBlur={e => {
        e.currentTarget.style.borderColor = "transparent"
        e.currentTarget.style.background = "transparent"
      }}
    />
  )
}