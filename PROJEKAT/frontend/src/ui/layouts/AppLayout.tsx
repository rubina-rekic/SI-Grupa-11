import type { PropsWithChildren } from "react"
import { toReadableUtc } from "../../shared/utils/dateTime"

interface AppLayoutProps extends PropsWithChildren {
  status: string
  initializedAtUtc: string
}

export function AppLayout({ status, initializedAtUtc, children }: AppLayoutProps) {
  return (
    <div className="app-shell">
      <header className="app-shell__header">
        <p className="app-shell__title">PostRoute Frontend Skeleton</p>
        <p className="app-shell__status">{status}</p>
        <p className="app-shell__timestamp">Initialized: {toReadableUtc(initializedAtUtc)}</p>
      </header>
      <main className="app-shell__main">{children}</main>
    </div>
  )
}
