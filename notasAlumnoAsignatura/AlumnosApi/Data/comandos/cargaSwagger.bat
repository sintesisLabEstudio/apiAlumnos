@echo off
setlocal EnableExtensions
title Abrir Swagger - AlumnosApi
set "URL=http://localhost:50555/swagger"

echo ==========================================================
echo   ABRIR SWAGGER - ALUMNOSAPI
echo ==========================================================
echo.

set "HTTP=000"
for /f "tokens=2" %%h in ('curl.exe -s -m 5 -o nul -w "HTTP %%{http_code}" http://127.0.0.1:50555/api/alumnos/listar 2^>nul') do set "HTTP=%%h"

if not "%HTTP%"=="200" (
    echo La API no responde en el puerto 50555.
    echo Ejecuta primero ejecutarAPI.bat y vuelve a intentar.
    echo.
    pause
    exit /b 1
)

echo API respondiendo HTTP %HTTP%.
echo Abriendo: %URL%
start "" "%URL%"
echo.
pause
exit /b 0