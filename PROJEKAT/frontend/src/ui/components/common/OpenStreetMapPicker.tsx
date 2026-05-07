import { useEffect, useState } from "react"
import { MapContainer as LeafletMap, Marker, TileLayer, useMapEvents } from "react-leaflet"
import "leaflet/dist/leaflet.css"
import L, { type LeafletEvent, type LeafletMouseEvent } from "leaflet"

// Fix for default marker icon
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
  initialLat?: number
  initialLng?: number
  height?: string
}

function MapEvents({ onLocationSelect }: { onLocationSelect: (lat: number, lng: number) => void }) {
  useMapEvents({
    click: (e: LeafletMouseEvent) => {
      const { lat, lng } = e.latlng
      onLocationSelect(lat, lng)
    },
  })
  return null
}

export default function OpenStreetMapPicker({ 
  onLocationSelect, 
  initialLat = 43.8563, 
  initialLng = 18.4131,
  height = "400px" 
}: OpenStreetMapPickerProps) {
  const [position, setPosition] = useState<[number, number]>([initialLat, initialLng])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Simulate loading time for better UX
    const timer = setTimeout(() => {
      setLoading(false)
    }, 500)
    return () => clearTimeout(timer)
  }, [])

  const handleMapClick = (lat: number, lng: number) => {
    setPosition([lat, lng])
    onLocationSelect(lat, lng)
  }

  const handleMarkerDrag = (e: LeafletEvent) => {
    const marker = e.target as L.Marker
    const { lat, lng } = marker.getLatLng()
    setPosition([lat, lng])
    onLocationSelect(lat, lng)
  }

  if (loading) {
    return (
      <div 
        className="map-loading" 
        style={{ 
          height, 
          display: "flex", 
          alignItems: "center", 
          justifyContent: "center",
          backgroundColor: "#f8fafc",
          border: "1px solid #ccd9e6",
          borderRadius: "8px",
          color: "#64748b",
          fontSize: "0.9rem",
          flexDirection: "column",
          gap: "8px"
        }}
      >
        <div style={{ fontSize: "1.5rem" }}>🗺️</div>
        <div>Učitavanje mape...</div>
        <div style={{ fontSize: "0.8rem", opacity: 0.7 }}>Pričekajte trenutak</div>
      </div>
    )
  }

  return (
    <div className="map-container">
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
            eventHandlers={{
              dragend: handleMarkerDrag
            }}
          />
          <MapEvents onLocationSelect={handleMapClick} />
        </LeafletMap>
      </div>
      <div style={{ 
        marginTop: "12px", 
        fontSize: "0.85rem", 
        color: "#64748b",
        display: "flex",
        alignItems: "center",
        gap: "8px"
      }}>
        <span style={{ fontSize: "1rem" }}>💡</span>
        <div>
          <strong>Kliknite na mapu ili povucite marker</strong> da izaberete tačnu lokaciju sandučića.
        </div>
      </div>
    </div>
  )
}
