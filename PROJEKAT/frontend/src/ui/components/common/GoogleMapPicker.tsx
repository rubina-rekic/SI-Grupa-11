/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useRef, useState } from "react"

interface GoogleMapPickerProps {
  onLocationSelect: (lat: number, lng: number) => void
  initialLat?: number
  initialLng?: number
  height?: string
}

declare global {
  interface Window {
    google: any
  }
}

export default function GoogleMapPicker({ 
  onLocationSelect, 
  initialLat = 43.8563, 
  initialLng = 18.4131,
  height = "400px" 
}: GoogleMapPickerProps) {
  const mapRef = useRef<HTMLDivElement>(null)
  const mapInstanceRef = useRef<any>(null)
  const markerRef = useRef<any>(null)
  const [isLoaded, setIsLoaded] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const checkGoogleMaps = () => {
      try {
        if (window.google && window.google.maps) {
          setIsLoaded(true)
          setLoading(false)
          setError(null)
        } else {
          // Čekajmo da se Google Maps učita
          setTimeout(checkGoogleMaps, 100)
        }
      } catch {
        setError("Google Maps API nije dostupan. Provjerite internet konekciju.")
        setLoading(false)
      }
    }

    checkGoogleMaps()
  }, [])

  useEffect(() => {
    if (!isLoaded || !mapRef.current) return

    try {
      const map = new window.google.maps.Map(mapRef.current, {
        center: { lat: initialLat, lng: initialLng },
        zoom: 15,
        mapTypeControl: true,
        streetViewControl: true,
        fullscreenControl: true,
        zoomControl: true,
        styles: [
          {
            featureType: "poi",
            elementType: "labels",
            stylers: [{ visibility: "off" }]
          }
        ]
      })

      // Kreiranje markera
      const marker = new window.google.maps.Marker({
        position: { lat: initialLat, lng: initialLng },
        map: map,
        draggable: true,
        title: "Lokacija sandučića",
        animation: window.google.maps.Animation.DROP
      })

      // Event listener za klik na mapu
      map.addListener("click", (e: any) => {
        const lat = e.latLng.lat()
        const lng = e.latLng.lng()
        marker.setPosition({ lat, lng })
        marker.setAnimation(window.google.maps.Animation.DROP)
        onLocationSelect(lat, lng)
      })

      // Event listener za pomeranje markera
      marker.addListener("dragend", (e: any) => {
        const lat = e.latLng.lat()
        const lng = e.latLng.lng()
        onLocationSelect(lat, lng)
      })

      mapInstanceRef.current = map
      markerRef.current = marker

    } catch (err) {
      setError("Greška pri inicijalizaciji mape. Molimo osvježite stranicu.")
      console.error("Google Maps initialization error:", err)
    }
  }, [isLoaded, initialLat, initialLng, onLocationSelect])

  // Update marker position when props change
  useEffect(() => {
    if (markerRef.current && mapInstanceRef.current) {
      markerRef.current.setPosition({ lat: initialLat, lng: initialLng })
      mapInstanceRef.current.setCenter({ lat: initialLat, lng: initialLng })
    }
  }, [initialLat, initialLng])

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

  if (error) {
    return (
      <div 
        className="map-error" 
        style={{ 
          height, 
          display: "flex", 
          alignItems: "center", 
          justifyContent: "center",
          backgroundColor: "#fef2f2",
          border: "1px solid #fecaca",
          borderRadius: "8px",
          color: "#dc2626",
          fontSize: "0.9rem",
          flexDirection: "column",
          gap: "8px",
          padding: "20px"
        }}
      >
        <div style={{ fontSize: "1.5rem" }}>⚠️</div>
        <div>{error}</div>
        <button
          onClick={() => window.location.reload()}
          style={{
            padding: "8px 16px",
            backgroundColor: "#dc2626",
            color: "white",
            border: "none",
            borderRadius: "6px",
            cursor: "pointer",
            fontSize: "0.8rem"
          }}
        >
          Osvježi stranicu
        </button>
      </div>
    )
  }

  return (
    <div className="map-container">
      <div 
        ref={mapRef} 
        style={{ 
          height,
          borderRadius: "8px",
          border: "2px solid #2563a8",
          overflow: "hidden",
          boxShadow: "0 4px 12px rgba(37, 99, 168, 0.15)"
        }} 
      />
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
