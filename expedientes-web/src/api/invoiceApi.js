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