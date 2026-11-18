@echo off
setlocal EnableExtensions EnableDelayedExpansion
title Estado de la conexion BD
set "SERVER=(localdb)\MSSQLLocalDB"
set "TMP=%TEMP%\estado_bd_alumnos.txt"

echo ==========================================================
echo   ESTADO DE LA CONEXION CON LA BASE DE DATOS ALUMNOS
echo ==========================================================
echo.

echo 1) INSTANCIA LOCALDB
for /f "tokens=1,2" %%a in ('sqllocaldb info MSSQLLocalDB 2^>nul ^| findstr /I "State:"') do echo    Estado: %%b
echo.

echo 2) ESTADO DE LA BD ALUMNOS
sqlcmd -S "%SERVER%" -E -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID('ALUMNOS') IS NULL SELECT 0 ELSE SELECT 1" > "%TMP%" 2>nul
set "ESTADO_BD="
for /f "usebackq delims=" %%e in ("%TMP%") do set "ESTADO_BD=%%e"
if "%ESTADO_BD%"=="1" (echo    CONECTADA) else (echo    NO CONECTADA)
echo.

echo 3) MODO DE ACCESO Y ARCHIVO .mdf
if not "%ESTADO_BD%"=="1" (
    echo    No disponible: la BD no esta conectada.
) else (
    sqlcmd -S "%SERVER%" -E -W -Q "SET NOCOUNT ON; SELECT d.user_access_desc AS Modo, f.physical_name AS Archivo FROM sys.databases d JOIN sys.master_files f ON d.database_id = f.database_id WHERE d.name = N'ALUMNOS' AND f.type_desc = 'ROWS'"
)
echo.

echo 4) REGISTROS
if not "%ESTADO_BD%"=="1" (
    echo    No disponibles: la BD no esta conectada.
) else (
    sqlcmd -S "%SERVER%" -E -W -Q "SET NOCOUNT ON; SELECT (SELECT COUNT(*) FROM [ALUMNOS].[dbo].[ALUMNO]) AS Alumnos, (SELECT COUNT(*) FROM [ALUMNOS].[dbo].[ASIGNATURA]) AS Asignaturas, (SELECT COUNT(*) FROM [ALUMNOS].[dbo].[NOTAS]) AS Notas"
)
echo.

echo 5) API EN EL PUERTO 50555
netstat -ano | findstr "LISTENING" | findstr ":50555" >nul
if errorlevel 1 (
    echo    No hay proceso escuchando en 50555.
) else (
    echo    Escuchando en 50555.
)
echo.

echo 6) RESPUESTA HTTP DE LA API
set "HTTP=000"
for /f "tokens=2" %%h in ('curl.exe -s -m 5 -o nul -w "HTTP %%{http_code}" http://127.0.0.1:50555/api/alumnos/listar 2^>nul') do set "HTTP=%%h"
echo    HTTP %HTTP%
echo.

echo ----------------------------------------------------------
if "%ESTADO_BD%"=="1" if "%HTTP%"=="200" (
    echo RESULTADO: TODO OK. La app Kotlin puede consumir la API.
) else (
    echo RESULTADO: REVISAR. Ejecuta cargarBaseDatos.bat y luego ejecutarAPI.bat.
)
echo ----------------------------------------------------------
echo.
pause