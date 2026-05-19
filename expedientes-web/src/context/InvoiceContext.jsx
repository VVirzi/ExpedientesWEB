import { createContext, useContext, useState } from "react"

const InvoiceContext = createContext(null)

export function InvoiceProvider({ children }) {
  const [selectedClient, setSelectedClient] = useState(null)
  const [invoiceResult, setInvoiceResult] = useState(null)

  return (
    <InvoiceContext.Provider value={{
      selectedClient,
      setSelectedClient,
      invoiceResult,
      setInvoiceResult
    }}>
      {children}
    </InvoiceContext.Provider>
  )
}

export function useInvoice() {
  const context = useContext(InvoiceContext)
  if (!context) throw new Error("useInvoice debe usarse dentro de InvoiceProvider")
  return context
}