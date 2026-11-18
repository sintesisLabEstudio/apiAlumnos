@echo off
setlocal
set "SERVER=(localdb)\MSSQLLocalDB"
set "TMP=%TEMP%\alumnos_estado.txt"

echo RUTA DEL ARCHIVO .mdf DE LA BASE DE DATOS ALUMNOS:

sqlcmd -S "%SERVER%" -E -h -1 -W -Q "SET NOCOUNT ON; IF DB_ID('ALUMNOS') IS NULL SELECT 0 ELSE SELECT 1" > "%TMP%" 2>nul
set "ESTADO="
for /f "usebackq delims=" %%e in ("%TMP%") do set "ESTADO=%%e"
if not "%ESTADO%"=="1" goto :nodesconectada

sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT physical_name FROM sys.database_files WHERE type_desc = 'ROWS'"
echo.
echo Presione cualquier tecla para cerrar esta ventana...
pause >nul
exit /b 0

:nodesconectada
echo La BD ALUMNOS no esta conectada.
echo Ejecuta cargarBaseDatos.bat (en Data\comandos) y vuelve a intentar.
echo.
pause
exit /b 1