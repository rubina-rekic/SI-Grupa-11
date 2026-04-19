interface LayerTagProps {
  label: string
}

export function LayerTag({ label }: LayerTagProps) {
  return <span className="layer-tag">{label}</span>
}
