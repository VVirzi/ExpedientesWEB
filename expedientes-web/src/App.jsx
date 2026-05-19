import { Routes, Route, Navigate } from "react-router-dom"
import ClientSelectionPage from "./pages/ClientSelectionPage"
import FileUploadPage from "./pages/FileUploadPage"
import InvoiceReviewPage from "./pages/InvoiceReviewPage"

function App() {
  return (
    <Routes>
      <Route path="/" element={<ClientSelectionPage />} />
      <Route path="/upload" element={<FileUploadPage />} />
      <Route path="/review" element={<InvoiceReviewPage />} />
      <Route path="*" element={<Navigate to="/" />} />
    </Routes>
  )
}

export default App