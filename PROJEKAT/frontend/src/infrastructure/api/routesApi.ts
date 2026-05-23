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
    isManuallyReordered: boolean;
    mailboxStatus: string;
}

export interface RouteResponse {
    id: string;
    postmanId: string;
    postmanName: string | null;
    date: string;
    plannedStartTime: string;
    plannedEndTime: string | null;
    totalDistanceKm: number;
    totalDurationMinutes: number;
    status: string;
    exceedsStandardTime: boolean;
    lastReorderedAt: string | null;
    lastReorderedBy: string | null;
    assignedAt: string | null;
    assignedBy: string | null;
    routeItems: RouteItemResponse[];
}

export interface ReorderItem {
    routeItemId: string;
    newOrder: number;
}

export interface GenerateRouteRequest {
    postmanId: string;
    date: string;
    plannedStartTime: string;
}

export interface AvailablePostmanResponse {
    id: string;
    fullName: string;
    username: string;
    email: string;
    isAvailable: boolean;
    isCurrentAssignee: boolean;
    unavailableReason: string | null;
}

export const routesApi = {
    getRouteDetails: async (routeId: string): Promise<RouteResponse> => {
        const response = await httpClient<RouteResponse>(`/api/routes/${routeId}`);
        
        if (response.error || !response.data) {
            throw response;
        }
        
        return response.data;
    },

    getMyAssignedRouteForToday: async (): Promise<RouteResponse | null> => {
        const response = await httpClient<RouteResponse | { message: string }>('/api/routes/my-assigned-route/today');
        
        if (response.error) {
            throw response;
        }

        if (!response.data) {
            return null;
        }

        // Check if response is a message (no route assigned)
        if ('message' in response.data) {
            return null;
        }

        return response.data as RouteResponse;
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
    },

    getAvailablePostmen: async (routeId: string): Promise<AvailablePostmanResponse[]> => {
        const response = await httpClient<AvailablePostmanResponse[]>(`/api/routes/${routeId}/available-postmen`);

        if (response.error || !response.data) {
            throw response;
        }

        return response.data;
    },

    assignRoute: async (routeId: string, postmanId: string): Promise<RouteResponse> => {
        const response = await httpClient<RouteResponse>(`/api/routes/${routeId}/assign`, {
            method: 'PUT',
            body: { postmanId }
        });

        if (response.error || !response.data) {
            throw response;
        }

        return response.data;
    },

    reorderRoute: async (routeId: string, items: ReorderItem[]): Promise<RouteResponse> => {
        const response = await httpClient<RouteResponse>(`/api/routes/${routeId}/reorder`, {
            method: 'PUT',
            body: { items }
        });

        if (response.error || !response.data) {
            throw response;
        }

        return response.data;
    }
};
