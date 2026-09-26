# ===== Tầng 1: "bếp" — biên dịch code =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/CourseHub.Domain/CourseHub.Domain.csproj src/CourseHub.Domain/
COPY src/CourseHub.Application/CourseHub.Application.csproj src/CourseHub.Application/
COPY src/CourseHub.Infrastructure/CourseHub.Infrastructure.csproj src/CourseHub.Infrastructure/
COPY src/CourseHub.API/CourseHub.API.csproj src/CourseHub.API/
RUN dotnet restore src/CourseHub.API/CourseHub.API.csproj

COPY src/ src/
RUN dotnet publish src/CourseHub.API/CourseHub.API.csproj -c Release -o /app/publish --no-restore

# ===== Tầng 2: "đĩa ăn" — chỉ chứa thứ cần để chạy =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

RUN mkdir -p /app/wwwroot/uploads /app/Logs && chown -R $APP_UID /app/wwwroot /app/Logs
USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "CourseHub.API.dll"]