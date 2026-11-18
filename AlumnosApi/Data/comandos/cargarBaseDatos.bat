@echo off
setlocal EnableExtensions
title Cargar base de datos ALUMNOS
set "SERVER=(localdb)\MSSQLLocalDB"
set "TMPRUTAS=%TEMP%\cargar_bd_rutas.txt"

echo ==========================================================
echo   CARGAR LA BASE DE DATOS REAL DESDE LA RUTA DEL MOTOR
echo ==========================================================
echo.

echo 1/5 Verificando LocalDB...
call :asegurar_localdb
if errorlevel 1 goto :fallo

echo 2/5 Preguntando al motor la ruta de los archivos...
sqlcmd -S "%SERVER%" -E -h -1 -W -s "|" -Q "SET NOCOUNT ON; SELECT SERVERPROPERTY('InstanceDefaultDataPath'), SERVERPROPERTY('InstanceDefaultLogPath')" > "%TMPRUTAS%"
for /f "usebackq tokens=1,2 delims=|" %%a in ("%TMPRUTAS%") do (
    set "RDATOS=%%a"
    set "RLOGS=%%b"
)
if not defined RDATOS goto :sinruta
for %%i in ("%RDATOS%ALUMNOS.mdf") do set "MDF=%%~fi"
for %%i in ("%RLOGS%ALUMNOS_log.ldf") do set "LDF=%%~fi"
echo     MDF: %MDF%
echo     LDF: %LDF%
echo.
if not exist "%MDF%" goto :nofiles
if not exist "%LDF%" goto :nofiles

echo 3/5 Cerrando la base temporal si estaba abierta...
sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS_RUTA_ACTUAL') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS_RUTA_ACTUAL SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS_RUTA_ACTUAL'; END" >nul 2>&1

echo 4/5 Desconectando la BD ALUMNOS actual (sin borrar archivos)...
sqlcmd -S "%SERVER%" -E -b -Q "IF DB_ID('ALUMNOS') IS NOT NULL BEGIN ALTER DATABASE ALUMNOS SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname=N'ALUMNOS'; END"
if errorlevel 1 goto :fallo

echo 5/5 Adjuntando los archivos reales y dejando MULTI_USER...
sqlcmd -S "%SERVER%" -E -b -Q "CREATE DATABASE ALUMNOS ON (FILENAME=N'%MDF%'), (FILENAME=N'%LDF%') FOR ATTACH;"
if errorlevel 1 goto :fallo
sqlcmd -S "%SERVER%" -E -b -Q "ALTER DATABASE ALUMNOS SET MULTI_USER;" >nul 2>&1

echo.
echo Base de datos cargada desde:
sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT physical_name FROM sys.database_files WHERE type_desc = 'ROWS'"
echo Tablas:
sqlcmd -S "%SERVER%" -E -d ALUMNOS -h -1 -W -Q "SET NOCOUNT ON; SELECT name FROM sys.tables ORDER BY name"
echo.
echo LISTO. La API conecta por nombre: Database=ALUMNOS.
echo.
pause
exit /b 0

:asegurar_localdb
sqllocaldb info MSSQLLocalDB >nul 2>&1
if errorlevel 1 sqllocaldb create MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if not errorlevel 1 exit /b 0
sqllocaldb start MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if not errorlevel 1 exit /b 0
echo El motor quedo desincronizado; recuperando la instancia...
powershell -NoProfile -Command "Get-CimInstance Win32_Process | Where-Object { $_.Name -eq 'sqlservr.exe' -and $_.CommandLine -like '*MSSQLLocalDB*' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }"
sqllocaldb stop MSSQLLocalDB >nul 2>&1
sqllocaldb start MSSQLLocalDB >nul 2>&1
sqllocaldb info MSSQLLocalDB 2>nul | findstr /I "State:.*Running" >nul
if errorlevel 1 exit /b 1
exit /b 0

:sinruta
echo ERROR: LocalDB no disponible o no se pudo consultar la ruta del motor.
echo.
pause
exit /b 1

:nofiles
echo ERROR: no estan los archivos reales en la ruta del motor.
echo Ejecuta crearBaseDatosAlumnos.bat para que el motor los cree.
echo.
pause
exit /b 1

:fallo
echo ERROR: no se pudo cargar la base de datos real.
echo Revisa los mensajes y vuelve a intentar.
echo.
pause
exit /b 1