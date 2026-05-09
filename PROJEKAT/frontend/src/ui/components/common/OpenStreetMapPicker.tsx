import { useEffect, useState, useRef } from "react"
import { MapContainer as LeafletMap, Marker, TileLayer, useMapEvents, useMap } from "react-leaflet"
import "leaflet/dist/leaflet.css"
import L, { type LeafletEvent, type LeafletMouseEvent } from "leaflet"

delete (L.Icon.Default.prototype as L.Icon.Default & { _getIconUrl?: unknown })._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  shadowSize: [41, 41]
})

interface OpenStreetMapPickerProps {
  onLocationSelect: (lat: number, lng: number) => void
  onAddressFound?: (address: string) => void
  initialLat?: number
  initialLng?: number
  height?: string
}

function MapEvents({ onLocationSelect }: { onLocationSelect: (lat: number, lng: number) => void }) {
  useMapEvents({
    click: (e: LeafletMouseEvent) => {
      const { lat, lng } = e.latlng
      void onLocationSelect(lat, lng)
    },
  })
  return null
}

function MapController({ center }: { center: [number, number] | null }) {
  const map = useMap()
  useEffect(() => {
    if (center) {
      map.flyTo(center, 17)
    }
  }, [center, map])
  return null
}

export default function OpenStreetMapPicker({
  onLocationSelect,
  onAddressFound,
  initialLat = 43.8563,
  initialLng = 18.4131,
  height = "400px"
}: OpenStreetMapPickerProps) {
  const [position, setPosition] = useState<[number, number]>([initialLat, initialLng])
  const [loading, setLoading] = useState(true)
  const [searchQuery, setSearchQuery] = useState("")
  const [searching, setSearching] = useState(false)
  const [searchError, setSearchError] = useState<string | null>(null)
  const [flyTo, setFlyTo] = useState<[number, number] | null>(null)
  const searchTimeout = useRef<ReturnType<typeof setTimeout> | null>(null)

  useEffect(() => {
    const timer = setTimeout(() => setLoading(false), 500)
    return () => clearTimeout(timer)
  }, [])

  const handleMapClick = async (lat: number, lng: number) => {
  setPosition([lat, lng])
  onLocationSelect(lat, lng)
  
  if (onAddressFound) {
    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lng}&format=json`,
        { headers: { "Accept-Language": "bs" } }
      )
      const data = await response.json()
      if (data?.display_name) {
        onAddressFound(data.display_name)
      }
    } catch {
      // Ignorisati greške reverse geocodinga
    }
  }
}

  const handleMarkerDrag = (e: LeafletEvent) => {
    const marker = e.target as L.Marker
    const { lat, lng } = marker.getLatLng()
    setPosition([lat, lng])
    onLocationSelect(lat, lng)
  }

  const handleSearch = async () => {
    const query = searchQuery.trim()
    if (!query) return

    setSearching(true)
    setSearchError(null)

    try {
      const response = await fetch(
        `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=1&countrycodes=ba`,
        { headers: { "Accept-Language": "bs" } }
      )
      const data = await response.json()

      if (data && data.length > 0) {
        const lat = parseFloat(data[0].lat)
        const lng = parseFloat(data[0].lon)
        setPosition([lat, lng])
        setFlyTo([lat, lng])
        onLocationSelect(lat, lng)
        setSearchError(null)
        if (onAddressFound) {
          onAddressFound(data[0].display_name)
        }
      } else {
        setSearchError("Adresa nije pronađena. Pokušajte s preciznijim unosom.")
      }
    } catch {
      setSearchError("Greška pri pretrazi. Provjerite internet konekciju.")
    } finally {
      setSearching(false)
    }
  }

  const handleSearchKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      e.preventDefault()
      if (searchTimeout.current) clearTimeout(searchTimeout.current)
      void handleSearch()
    }
  }

  if (loading) {
    return (
      <div style={{
        height, display: "flex", alignItems: "center", justifyContent: "center",
        backgroundColor: "#f8fafc", border: "1px solid #ccd9e6",
        borderRadius: "8px", color: "#64748b", fontSize: "0.9rem",
        flexDirection: "column", gap: "8px"
      }}>
        <div style={{ fontSize: "1.5rem" }}>🗺️</div>
        <div>Učitavanje mape...</div>
        <div style={{ fontSize: "0.8rem", opacity: 0.7 }}>Pričekajte trenutak</div>
      </div>
    )
  }

  return (
    <div>
      {/* Polje za pretragu adrese */}
      <div style={{ marginBottom: "8px", display: "flex", gap: "8px" }}>
        <input
          type="text"
          className="form-field__input"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          onKeyDown={handleSearchKeyDown}
          placeholder="Ukucajte adresu i pritisnite Enter ili kliknite Traži..."
          style={{ flex: 1 }}
        />
        <button
          type="button"
          className="btn btn--primary"
          onClick={() => void handleSearch()}
          disabled={searching || !searchQuery.trim()}
          style={{ padding: "8px 16px", whiteSpace: "nowrap" }}
        >
          {searching ? "Traženje..." : "Traži"}
        </button>
      </div>

      {searchError && (
        <div style={{
          marginBottom: "8px", padding: "8px 12px",
          backgroundColor: "#fef2f2", border: "1px solid #fecaca",
          borderRadius: "6px", fontSize: "0.85rem", color: "#b91c1c"
        }}>
          ⚠️ {searchError}
        </div>
      )}

      {/* Mapa */}
      <div style={{ height, borderRadius: "8px", overflow: "hidden", border: "2px solid #2563a8" }}>
        <LeafletMap
          center={position}
          zoom={15}
          style={{ height: "100%", width: "100%" }}
          scrollWheelZoom={true}
        >
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <Marker
            position={position}
            draggable={true}
            eventHandlers={{ dragend: handleMarkerDrag }}
          />
          <MapEvents onLocationSelect={handleMapClick} />
          <MapController center={flyTo} />
        </LeafletMap>
      </div>

      <div style={{
        marginTop: "8px", fontSize: "0.85rem", color: "#64748b",
        display: "flex", alignItems: "center", gap: "8px"
      }}>
        <span>💡</span>
        <div>
          <strong>Pretražite adresu</strong> ili <strong>kliknite direktno na mapu</strong> da označite lokaciju sandučića. Marker možete i povući.
        </div>
      </div>
    </div>
  )
}