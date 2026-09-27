# ===== Tầng 1: "nhà in" — build Angular bằng đúng Node 14 =====
FROM node:14-alpine AS build
WORKDIR /app

RUN npm install -g npm@8

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build -- --configuration production

# ===== Tầng 2: "giá sách" — nginx phát file tĩnh =====
FROM nginx:1.27-alpine
COPY nginx.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/dist/frontend /usr/share/nginx/html
EXPOSE 80