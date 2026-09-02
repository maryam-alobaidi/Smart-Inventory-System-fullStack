# مرحلة 1: بناء تطبيق Angular
FROMM node:20 AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

# مرحلة 2: خدمة الملفات الجاهزة عبر nginx
FROM nginx:alpine
COPY --from=build /app/dist/SmartInventoryUI/browser /usr/share/nginx/html
EXPOSE 80