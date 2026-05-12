import { useEffect } from "react"
import L from "leaflet"
import { useMap } from "react-leaflet"
import "leaflet-routing-machine"

interface LeafletRoutingMachineProps {
  waypoints: Array<[number, number]>
}

export function LeafletRoutingMachine({ waypoints }: LeafletRoutingMachineProps) {
  const map = useMap()

  useEffect(() => {
    if (waypoints.length < 2) {
      return
    }

    const plan = L.Routing.plan(waypoints.map(([latitude, longitude]) => L.latLng(latitude, longitude)), {
      addWaypoints: false,
      draggableWaypoints: false,
      routeWhileDragging: false,
      createMarker: () => false,
    })

    const control = L.Routing.control({
      plan,
      router: L.Routing.osrmv1({
        serviceUrl: "https://router.project-osrm.org/route/v1",
      }),
      addWaypoints: false,
      fitSelectedRoutes: true,
      routeWhileDragging: false,
      show: false,
      showAlternatives: false,
      lineOptions: {
        extendToWaypoints: true,
        missingRouteTolerance: 0,
        styles: [{ color: "#2563a8", opacity: 0.9, weight: 5 }],
      },
    }).addTo(map)

    return () => {
      map.removeControl(control)
    }
  }, [map, waypoints])

  return null
}
