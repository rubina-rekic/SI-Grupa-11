import React, { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, Polyline } from 'react-leaflet';
import L from 'leaflet';
import type { RouteItemResponse } from '../../../infrastructure/api/routesApi';
import './RouteMap.css';

interface RouteMapProps {
    items: RouteItemResponse[];
}

// Custom icon factory
const createNumberedIcon = (number: number, status: string) => {
    const statusLower = status.toLowerCase();
    let backgroundColor = '#999';
    
    if (statusLower === 'obrađen' || statusLower === 'obrađeno') {
        backgroundColor = '#4caf50';
    } else if (statusLower === 'nedostupan') {
        backgroundColor = '#ff5252';
    }

    const html = `
        <div class="leaflet-custom-pin" style="background-color: ${backgroundColor}">
            <span class="pin-number">${number}</span>
        </div>
    `;

    return L.divIcon({
        html,
        className: 'custom-marker-icon',
        iconSize: [32, 32],
        iconAnchor: [16, 32],
        popupAnchor: [0, -32],
    });
};

export const RouteMap: React.FC<RouteMapProps> = ({ items }) => {
    const [mapCenter, setMapCenter] = useState<[number, number]>([43.85, 18.41]);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        if (items.length > 0) {
            // Calculate center point
            const avgLat = items.reduce((sum, item) => sum + item.latitude, 0) / items.length;
            const avgLng = items.reduce((sum, item) => sum + item.longitude, 0) / items.length;
            setMapCenter([avgLat, avgLng]);
        }
        setLoaded(true);
    }, [items]);

    // Create polyline points (route line)
    const routePoints: [number, number][] = items.map((item) => [item.latitude, item.longitude]);

    if (!loaded) {
        return <div className="route-map loading">Učitavanje mape...</div>;
    }

    return (
        <div className="route-map">
            <MapContainer
                center={mapCenter}
                zoom={13}
                style={{ height: '100%', width: '100%', borderRadius: '8px' }}
                scrollWheelZoom={true}
            >
                <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                />

                {/* Route line connecting all points */}
                {routePoints.length > 1 && (
                    <Polyline
                        positions={routePoints}
                        color="#1976d2"
                        weight={3}
                        opacity={0.7}
                        dashArray="5, 5"
                    />
                )}

                {/* Markers for each mailbox */}
                {items.map((item, index) => (
                    <Marker
                        key={item.id}
                        position={[item.latitude, item.longitude]}
                        icon={createNumberedIcon(index + 1, item.status)}
                    >
                        <Popup>
                            <div className="popup-content">
                                <strong className="popup-address">{item.address}</strong>
                                <div className="popup-details">
                                    <span>Redni broj: {index + 1}</span>
                                    <span>Vrijeme: {formatTime(item.estimatedArrivalTime)}</span>
                                    <span className="popup-status">
                                        Status: <strong>{formatStatus(item.status)}</strong>
                                    </span>
                                </div>
                            </div>
                        </Popup>
                    </Marker>
                ))}
            </MapContainer>
        </div>
    );
};

const formatTime = (timeString: string): string => {
    if (!timeString) return '--:--';
    const parts = timeString.split(':');
    if (parts.length >= 2) {
        return `${parts[0]}:${parts[1]}`;
    }
    return timeString;
};

const formatStatus = (status: string): string => {
    const statusLower = status.toLowerCase();
    if (statusLower === 'obrađen' || statusLower === 'obrađeno') return 'Obrađen';
    if (statusLower === 'nedostupan') return 'Nedostupan';
    if (statusLower === 'planirano') return 'Čeka';
    return status;
};
