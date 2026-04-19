import { environment } from "../../infrastructure/config/environment"
import { LayerTag } from "../components/common/LayerTag"

const layerNotes = [
  {
    title: "UI / Presentation",
    description: "Pages, layouts, and reusable components for rendering and interaction.",
  },
  {
    title: "Application",
    description: "Hooks and orchestration utilities that coordinate feature flow.",
  },
  {
    title: "Infrastructure",
    description: "API client placeholders, routing setup, and storage integration.",
  },
]

export function StructureOverviewPage() {
  return (
    <section className="overview">
      <h1 className="overview__title">Initial PostRoute Structure</h1>
      <p className="overview__subtitle">
        This frontend is an architecture-first skeleton. No Product Backlog Item features are implemented.
      </p>
      <p className="overview__meta">API base URL placeholder: {environment.apiBaseUrl}</p>

      <div className="overview__grid">
        {layerNotes.map((layer) => (
          <article key={layer.title} className="overview__card">
            <LayerTag label={layer.title} />
            <p>{layer.description}</p>
          </article>
        ))}
      </div>
    </section>
  )
}
