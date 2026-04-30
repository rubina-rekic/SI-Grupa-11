# ─── Stage 1: Build React app ────────────────────────────────────────────────
FROM node:24-alpine AS build
WORKDIR /app

# Install dependencies first (cached layer — rebuilds only if lock file changes)
COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# VITE_API_BASE_URL is empty so Axios uses same-origin requests.
# Nginx then intercepts /api/* and proxies them to the backend container.
ARG VITE_API_BASE_URL=""
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN npm run build

# ─── Stage 2: Serve with Nginx ───────────────────────────────────────────────
FROM nginx:alpine AS runtime

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
