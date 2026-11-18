@echo off
REM Levanta la BD ALUMNOS usando el script original del profesor.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0crearBaseDatosAlumnos.ps1"
echo.
pause
