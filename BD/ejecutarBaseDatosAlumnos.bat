@echo off
echo ==========================================
echo 1. Ejecutando script de Base de Datos...
echo ==========================================

sqlcmd -S "(localdb)\MSSQLLocalDB" -i "basedatosAlumnos_creacion.sql"

echo.
echo ==========================================
echo 2. Iniciando la API de Alumnos (.NET)...
echo ==========================================

REM Sube un nivel desde BD para quedar en la carpeta AlumnosApi
cd /d "%~dp0.."

dotnet run