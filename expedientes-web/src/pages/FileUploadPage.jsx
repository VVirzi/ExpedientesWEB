import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"
import { processInvoices } from "../api/invoiceApi"
import PageTransition from "../components/common/PageTransition"


export default function FileUploadPage() {
  const { selectedClient, setInvoiceResult } = useInvoice()
  const navigate = useNavigate()

  const [invoicesFile, setInvoicesFile] = useState(null)
  const [metadataFile, setMetadataFile] = useState(null)
  const [anmatFile, setAnmatFile] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [transitionTrigger, setTransitionTrigger] = useState(0)

  useEffect(() => {
    if (!selectedClient) {
      navigate("/")
    }
  }, [selectedClient])

  if (!selectedClient) return null

  const needsAnmat = selectedClient.requiredFiles.includes("anmat")

  async function handleSubmit() {
    if (!invoicesFile || !metadataFile) {
      setError("Debés cargar los archivos obligatorios.")
      return
    }
    if (needsAnmat && !anmatFile) {
      setError("Este cliente requiere el archivo ANMAT.")
      return
    }

    setLoading(true)
    setError(null)

    try {
      const result = await processInvoices(invoicesFile, metadataFile, anmatFile)
      setInvoiceResult(result)
      setTransitionTrigger(t => t + 1)
    } catch (err) {
      setError("Error al procesar los archivos. Verificá que sean correctos.")
      setLoading(false)
    }
  }

  return (
    <div style={{
      minHeight: "100vh",
      display: "flex",
      flexDirection: "column",
      alignItems: "center",
      justifyContent: "center",
      padding: "2rem",
      background: "var(--bg)"
    }}>
      <PageTransition
        trigger={transitionTrigger}
        onComplete={() => navigate("/review")}
      />

      <div style={{
        width: "100%",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        transition: "opacity 0.35s ease",
        opacity: transitionTrigger > 0 ? 0 : 1
      }}>
        <div
          className="clay"
          style={{
            width: "100%",
            maxWidth: "480px",
            padding: "2.5rem",
            borderRadius: "1.25rem"
          }}
        >
          <h1 style={{
            fontSize: "1.5rem",
            fontWeight: 700,
            color: "var(--text)",
            marginBottom: "0.25rem",
            textShadow: "2px 2px 6px rgba(160,150,200,0.5), -1px -1px 3px rgba(255,255,255,0.8)"
          }}>
            Cargar Archivos
          </h1>
          <p style={{
            fontSize: "0.85rem",
            color: "var(--muted)",
            marginBottom: "2rem"
          }}>
            Cliente: {selectedClient.name}
          </p>

          <div style={{ display: "flex", flexDirection: "column", gap: "1.25rem" }}>
            <FileInput
              label="Archivo de Facturas"
              accept=".xls,.xlsx"
              onChange={e => setInvoicesFile(e.target.files[0])}
              fileName={invoicesFile?.name}
            />
            <FileInput
              label="Archivo de Metadata"
              accept=".xls,.xlsx"
              onChange={e => setMetadataFile(e.target.files[0])}
              fileName={metadataFile?.name}
            />
            {needsAnmat && (
              <FileInput
                label="Archivo ANMAT"
                accept=".txt"
                onChange={e => setAnmatFile(e.target.files[0])}
                fileName={anmatFile?.name}
              />
            )}
          </div>

          {error && (
            <div style={{
              marginTop: "1rem",
              padding: "0.75rem 1rem",
              background: "rgba(255,80,80,0.1)",
              border: "1px solid rgba(255,80,80,0.3)",
              borderRadius: "0.75rem",
              fontSize: "0.85rem",
              color: "#cc3333"
            }}>
              {error}
            </div>
          )}

          <div style={{ display: "flex", gap: "0.75rem", marginTop: "2rem" }}>
            <button
              onClick={() => navigate("/")}
              className="btn-secondary"
              style={{ flex: 1 }}
            >
              Volver
            </button>
            <button
              onClick={handleSubmit}
              disabled={loading}
              className="btn-primary"
              style={{ flex: 1 }}
            >
              {loading ? "Procesando..." : "Procesar"}
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

function FileInput({ label, accept, onChange, fileName }) {
  return (
    <div>
      <label style={{
        display: "block",
        fontSize: "0.8rem",
        fontWeight: 500,
        color: "var(--muted)",
        marginBottom: "0.4rem"
      }}>
        {label} *
      </label>
      <label
        style={{
          display: "flex",
          alignItems: "center",
          gap: "0.75rem",
          padding: "0.6rem 1rem",
          borderRadius: "0.75rem",
          border: "none",
          cursor: "pointer",
          background: "#E8E4F5",
          boxShadow: "inset 3px 3px 7px rgba(160,150,200,0.4), inset -2px -2px 5px rgba(255,255,255,0.7)",
          transition: "box-shadow 0.2s"
        }}
        onMouseEnter={e => e.currentTarget.style.boxShadow = "inset 4px 4px 9px rgba(140,120,200,0.5), inset -3px -3px 6px rgba(255,255,255,0.8)"}
        onMouseLeave={e => e.currentTarget.style.boxShadow = "inset 3px 3px 7px rgba(160,150,200,0.4), inset -2px -2px 5px rgba(255,255,255,0.7)"}
      >
        <span style={{
          fontSize: "0.8rem",
          fontWeight: 500,
          color: "var(--accent)",
          whiteSpace: "nowrap"
        }}>
          Seleccionar
        </span>
        <span style={{
          fontSize: "0.8rem",
          color: "var(--muted)",
          overflow: "hidden",
          textOverflow: "ellipsis",
          whiteSpace: "nowrap"
        }}>
          {fileName ?? "Ningún archivo seleccionado"}
        </span>
        <input type="file" accept={accept} onChange={onChange} style={{ display: "none" }} />
      </label>
    </div>
  )
}