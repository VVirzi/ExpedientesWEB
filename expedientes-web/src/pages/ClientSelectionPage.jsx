import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"

const CLIENTS = [
  {
    id: "ClientA",
    name: "Cliente 1",
    description: "Exporta QR",
    requiredFiles: ["invoices", "metadata"]
  },
  {
    id: "ClientB",
    name: "Cliente 2",
    description: "Exporta TXT + TXT",
    requiredFiles: ["invoices", "metadata"]
  },
  {
    id: "ClientC",
    name: "Cliente 3",
    description: "Exporta TXT",
    requiredFiles: ["invoices", "metadata", "anmat"]
  }
]

export default function ClientSelectionPage() {
  const { setSelectedClient } = useInvoice()
  const navigate = useNavigate()

  function handleClientSelect(client) {
    setSelectedClient(client)
    navigate("/upload")
  }

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col items-center justify-center p-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-2">Expedientes Web</h1>
      <p className="text-gray-500 mb-10">Seleccioná el cliente para comenzar</p>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 w-full max-w-3xl">
        {CLIENTS.map(client => (
          <button
            key={client.id}
            onClick={() => handleClientSelect(client)}
            className="bg-white border border-gray-200 rounded-2xl p-6 shadow-sm hover:shadow-md hover:border-blue-400 transition-all text-left"
          >
            <h2 className="text-lg font-semibold text-gray-800 mb-1">{client.name}</h2>
            <p className="text-sm text-gray-500">{client.description}</p>
          </button>
        ))}
      </div>
    </div>
  )
}