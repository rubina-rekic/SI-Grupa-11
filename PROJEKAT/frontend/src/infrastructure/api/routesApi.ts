import { httpClient } from './httpClient';

export interface RouteItemResponse {
    id: string;
    mailboxId: string;
    address: string;
    latitude: number;
    longitude: number;
    order: number;
    estimatedArrivalTime: string;
    priority: string;
    status: string;
}

export interface RouteResponse {
    id: string;
    postmanId: string;
    date: string;
    plannedStartTime: string;
    plannedEndTime: string;
    totalDistanceKm: number;
    totalDurationMinutes: number;
    status: string;
    exceedsStandardTime: boolean;
    routeItems: RouteItemResponse[];
}

export interface GenerateRouteRequest {
    postmanId: string;
    date: string;
    plannedStartTime: string;
}

export const routesApi = {
    getRouteDetails: async (routeId: string): Promise<RouteResponse> => {
        const response = await httpClient<RouteResponse>(`/api/routes/${routeId}`);
        
        if (response.error || !response.data) {
            throw response;
        }
        
        return response.data;
    },

    generateRoute: async (request: GenerateRouteRequest): Promise<RouteResponse> => {
        const response = await httpClient<RouteResponse>('/api/routes/generate', { 
            method: 'POST', 
            body: request 
        });
        
        if (response.error || !response.data) {
            throw response;
        }
        
        return response.data;
    }
};