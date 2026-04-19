import { useBootstrapStatus } from "../application/hooks/useBootstrapStatus"
import { AppRouter } from "../infrastructure/routing/AppRouter"
import { AppLayout } from "../ui/layouts/AppLayout"

function App() {
  const status = useBootstrapStatus()

  return (
    <AppLayout status={status.message} initializedAtUtc={status.initializedAtUtc}>
      <AppRouter />
    </AppLayout>
  )
}

export default App
