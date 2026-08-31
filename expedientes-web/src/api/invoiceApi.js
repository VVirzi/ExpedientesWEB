import axios from "axios"

const API_URL = "https://localhost:7249/api/invoices"

export async function processInvoices(invoicesFile, metadataFile, anmatFile) {
  const formData = new FormData()
  formData.append("invoicesFile", invoicesFile)
  formData.append("metadataFile", metadataFile)
  if (anmatFile) formData.append("anmatFile", anmatFile)

  const response = await axios.post(`${API_URL}/process`, formData, {
    headers: { "Content-Type": "multipart/form-data" }
  })

  return response.data
}

export async function exportInvoices(clientId, exportType, invoiceResult) {
  const response = await axios.post(
    `${API_URL}/export`,
    {
      clientId,
      exportType,
      result: invoiceResult
    },
    { responseType: "blob" }
  )

  const extension = exportType === "pdf" ? "pdf" : "txt"
  const url = window.URL.createObjectURL(new Blob([response.data]))
  const link = document.createElement("a")
  link.href = url
  link.setAttribute("download", `export_${clientId}_${exportType}.${extension}`)
  document.body.appendChild(link)
  link.click()
  link.remove()
  window.URL.revokeObjectURL(url)
}