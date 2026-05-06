import { Toaster } from "sonner"
import { AppRouter } from "../infrastructure/routing/AppRouter"
import "../styles/layout.css"

function App() {
  return (
    <>
      <AppRouter />
      <Toaster position="top-right" richColors closeButton />
    </>
  )
}

export default App
