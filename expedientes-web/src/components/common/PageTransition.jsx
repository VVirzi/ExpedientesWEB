import { useEffect, useState } from "react"

export default function PageTransition({ trigger, onComplete }) {
  const [phase, setPhase] = useState("idle")
  const [accentPos, setAccentPos] = useState("-100%")
  const [bgPos, setBgPos] = useState("-100%")

  useEffect(() => {
    if (!trigger) return

    // Fase 1: desvanecer contenido
    setPhase("fade")

    // Fase 2: entrar el rectángulo verde desde la izquierda
    setTimeout(() => {
      setPhase("sweep")
      setAccentPos("-100%")
      setBgPos("-200%")

      // Siguiente frame: mover a posición final para disparar la transición
      requestAnimationFrame(() => {
        requestAnimationFrame(() => {
          setAccentPos("0%")
        })
      })
    }, 350)

    // Fase 3: el verde sale por la derecha y entra el fondo
    setTimeout(() => {
      setAccentPos("100%")
      setBgPos("0%")
    }, 850)

    // Fase 4: limpiar y navegar
    setTimeout(() => {
      setPhase("idle")
      onComplete?.()
    }, 1350)
  }, [trigger])

  return (
    <>
      {/* Fade del contenido */}
      {phase === "fade" && (
        <div style={{
          position: "fixed",
          top: 0, left: 0,
          width: "100vw", height: "100vh",
          background: "var(--bg)",
          zIndex: 9997,
          opacity: 0,
          animation: "fadeIn 0.35s ease forwards",
          pointerEvents: "none"
        }} />
      )}

      {/* Rectángulo verde menta */}
      {phase === "sweep" && (
        <div style={{
          position: "fixed",
          top: 0, left: 0,
          width: "100vw", height: "100vh",
          background: "var(--accent)",
          zIndex: 9999,
          transform: `translateX(${accentPos})`,
          transition: "transform 0.5s cubic-bezier(0.4, 0, 0.2, 1)",
          pointerEvents: "none"
        }} />
      )}

      {/* Rectángulo fondo que sigue */}
      {phase === "sweep" && (
        <div style={{
          position: "fixed",
          top: 0, left: 0,
          width: "100vw", height: "100vh",
          background: "var(--bg)",
          zIndex: 9998,
          transform: `translateX(${bgPos})`,
          transition: "transform 0.5s cubic-bezier(0.4, 0, 0.2, 1)",
          pointerEvents: "none"
        }} />
      )}

      <style>{`
        @keyframes fadeIn {
          from { opacity: 0; }
          to { opacity: 1; }
        }
      `}</style>
    </>
  )
}