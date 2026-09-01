import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { useInvoice } from "../context/InvoiceContext"

const CLIENTS = [
  {
    id: "ClientA",
    name: "Cliente 1",
    description: "Exporta QR",
    requiredFiles: ["invoices", "metadata"],
    icon: "◈"
  },
  {
    id: "ClientB",
    name: "Cliente 2",
    description: "Exporta TXT + TXT",
    requiredFiles: ["invoices"],
    icon: "◉"
  },
  {
    id: "ClientC",
    name: "Cliente 3",
    description: "Exporta TXT",
    requiredFiles: ["invoices", "metadata", "anmat"],
    icon: "◎"
  }
]

export default function ClientSelectionPage() {
  const { setSelectedClient } = useInvoice()
  const navigate = useNavigate()
  const [selectedId, setSelectedId] = useState(null)
  const [animating, setAnimating] = useState(false)
  const [mounted, setMounted] = useState(false)
  const [visibleCards, setVisibleCards] = useState([])

  useEffect(() => {
    setTimeout(() => setMounted(true), 100)

    CLIENTS.forEach((_, index) => {
      setTimeout(() => {
        setVisibleCards(prev => [...prev, index])
      }, 400 + index * 200)
    })
  }, [])

  function handleClientSelect(client) {
    if (animating) return
    setAnimating(true)
    setSelectedId(client.id)

    setTimeout(() => {
      setSelectedClient(client)
      navigate("/upload")
    }, 800)
  }

  return (
    <div
      className="min-h-screen flex flex-col items-center justify-center p-8 relative overflow-hidden"
      style={{
        background: "var(--bg)",
        opacity: mounted ? 1 : 0,
        transform: mounted ? "scale(1)" : "scale(0.97)",
        transition: "opacity 0.8s ease, transform 0.8s ease"
      }}
    >
      {/* Background glow */}
      <div style={{
        position: "absolute",
        top: "30%",
        left: "50%",
        transform: "translateX(-50%)",
        width: "600px",
        height: "300px",
        background: "radial-gradient(ellipse, rgba(107,79,216,0.08) 0%, transparent 70%)",
        pointerEvents: "none"
      }} />

      {/* Header */}
      <div style={{
        transition: "opacity 0.3s ease",
        opacity: selectedId ? 0 : 1,
        marginBottom: "3rem",
        textAlign: "center"
      }}>
        <h1 style={{
          fontSize: "2rem",
          fontWeight: 700,
          color: "var(--text)",
          letterSpacing: "-0.02em",
          marginBottom: "0.5rem",
          textShadow: "2px 2px 6px rgba(160,150,200,0.5), -1px -1px 3px rgba(255,255,255,0.8)"
        }}>
          Expedientes Web
        </h1>
        <p style={{ color: "var(--muted)", fontSize: "0.95rem" }}>
          Seleccioná el cliente para comenzar
        </p>
      </div>

      {/* Cards */}
      <div style={{ display: "flex", gap: "1.5rem", position: "relative" }}>
        {CLIENTS.map((client, index) => {
          const isSelected = selectedId === client.id
          const isOther = selectedId && !isSelected
          const isVisible = visibleCards.includes(index)

          return (
            <div
              key={client.id}
              onClick={() => handleClientSelect(client)}
              style={{
                padding: "1.75rem 1.5rem",
                borderRadius: isSelected ? "0px" : "1.25rem",
                cursor: animating ? "default" : "pointer",
                position: isSelected ? "fixed" : "relative",
                top: isSelected ? 0 : "auto",
                left: isSelected ? 0 : "auto",
                width: isSelected ? "100vw" : "200px",
                height: isSelected ? "100vh" : "auto",
                zIndex: isSelected ? 100 : 1,
                transition: [
                  "width 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                  "height 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                  "border-radius 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                  "opacity 0.4s ease",
                  "transform 0.4s cubic-bezier(0.4, 0, 0.2, 1)",
                  "top 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                  "left 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                ].join(", "),
                opacity: isOther ? 0 : isVisible ? 1 : 0,
                transform: isVisible && !isOther ? "translateY(0)" : "translateY(16px)",
                background: "linear-gradient(145deg, #F4F1FC, #EDE9F8)",
                border: "none",
                boxShadow: isSelected
                  ? "none"
                  : "6px 6px 14px rgba(180,170,220,0.7), -6px -6px 14px rgba(220,216,240,0.7), inset 0 2px 4px rgba(255,255,255,0.6), inset 0 -2px 4px rgba(160,150,200,0.3)",
                display: "flex",
                flexDirection: isSelected ? "row" : "column",
                alignItems: isSelected ? "center" : "flex-start",
                justifyContent: isSelected ? "center" : "flex-start",
                gap: isSelected ? "1rem" : "0",
                userSelect: "none"
              }}
              onMouseEnter={e => {
                if (!animating && !selectedId) {
                  e.currentTarget.style.transform = "translateY(-6px) scale(1.02)"
                  e.currentTarget.style.boxShadow = "8px 8px 18px rgba(140,120,210,0.5), -6px -6px 14px rgba(220,216,240,0.8), inset 0 2px 4px rgba(255,255,255,0.6), inset 0 -2px 4px rgba(160,150,200,0.3)"
                }
              }}
              onMouseLeave={e => {
                if (!animating && !selectedId) {
                  e.currentTarget.style.transform = "translateY(0) scale(1)"
                  e.currentTarget.style.boxShadow = "6px 6px 14px rgba(180,170,220,0.7), -6px -6px 14px rgba(220,216,240,0.7), inset 0 2px 4px rgba(255,255,255,0.6), inset 0 -2px 4px rgba(160,150,200,0.3)"
                }
              }}
            >
              <div style={{
                fontSize: isSelected ? "2.5rem" : "1.75rem",
                marginBottom: isSelected ? 0 : "1rem",
                color: "var(--accent)",
                transition: "font-size 0.3s ease"
              }}>
                {client.icon}
              </div>
              <div>
                <h2 style={{
                  fontSize: "1rem",
                  fontWeight: 600,
                  color: "var(--text)",
                  marginBottom: "0.4rem"
                }}>
                  {client.name}
                </h2>
                <p style={{
                  fontSize: "0.8rem",
                  color: "var(--muted)"
                }}>
                  {client.description}
                </p>
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}