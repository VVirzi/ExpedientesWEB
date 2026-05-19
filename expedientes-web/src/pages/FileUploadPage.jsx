import { useState } from "react"
import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"
import { processInvoices } from "../api/invoiceApi"

export default function FileUploadPage() {
  const { selectedClient, setInvoiceResult } = useInvoice()
  const navigate = useNavigate()

  const [invoicesFile, setInvoicesFile] = useState(null)
  const [metadataFile, setMetadataFile] = useState(null)
  const [anmatFile, setAnmatFile] = useState(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)

  if (!selectedClient) {
    navigate("/")
    return null
  }

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
      navigate("/review")
    } catch (err) {
      setError("Error al procesar los archivos. Verificá que sean correctos.")
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center p-8">
      <div className="bg-white rounded-2xl shadow-sm border border-gray-200 w-full max-w-lg p-8">
        <h1 className="text-2xl font-bold text-gray-800 mb-1">Cargar Archivos</h1>
        <p className="text-sm text-gray-500 mb-8">Cliente: {selectedClient.name}</p>

        <div className="flex flex-col gap-5">
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
          <p className="mt-4 text-sm text-red-500 bg-red-50 border border-red-200 rounded-lg px-4 py-2">
            {error}
          </p>
        )}

        <div className="flex gap-3 mt-8">
          <button
            onClick={() => navigate("/")}
            className="flex-1 py-2 rounded-xl border border-gray-300 text-gray-600 hover:bg-gray-50 transition"
          >
            Volver
          </button>
          <button
            onClick={handleSubmit}
            disabled={loading}
            className="flex-1 py-2 rounded-xl bg-blue-600 text-white font-medium hover:bg-blue-700 disabled:opacity-50 transition"
          >
            {loading ? "Procesando..." : "Procesar"}
          </button>
        </div>
      </div>
    </div>
  )
}

function FileInput({ label, accept, onChange, fileName }) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">{label} *</label>
      <label className="flex items-center gap-3 border border-gray-300 rounded-xl px-4 py-2 cursor-pointer hover:border-blue-400 transition">
        <span className="text-sm text-blue-600 font-medium whitespace-nowrap">Seleccionar</span>
        <span className="text-sm text-gray-400 truncate">
          {fileName ?? "Ningún archivo seleccionado"}
        </span>
        <input type="file" accept={accept} onChange={onChange} className="hidden" />
      </label>
    </div>
  )
}