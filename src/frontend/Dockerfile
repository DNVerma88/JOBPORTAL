# ── Build stage ───────────────────────────────────────────────
FROM node:22-alpine AS build
WORKDIR /app

# Install deps with cache mount
COPY package.json package-lock.json ./
RUN npm ci --legacy-peer-deps

# Copy source and build
COPY . .
ARG VITE_API_URL=/api/v1
ENV VITE_API_URL=${VITE_API_URL}
ARG VITE_TENANT_ID=00000000-0000-0000-0000-000000000001
ENV VITE_TENANT_ID=${VITE_TENANT_ID}
RUN npm run build

# ── Runtime stage (nginx) ─────────────────────────────────────
FROM nginx:1.27-alpine AS runtime

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
