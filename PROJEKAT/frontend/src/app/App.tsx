import { Toaster } from "sonner"
import { AppRouter } from "../infrastructure/routing/AppRouter"

function App() {
  return (
    <>
      <AppRouter />
      <Toaster position="top-right" richColors closeButton />
    </>
  )
}

export default App
