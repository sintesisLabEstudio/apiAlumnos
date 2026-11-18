@echo off
setlocal
REM ===================================================================
REM EJECUTAR LA API SIN VISUAL STUDIO
REM Se ubica en Data\comandos: %~dp0..\.. es la carpeta AlumnosApi.
REM Levanta Kestrel en HTTP puerto 50555 (el que consume la app Kotlin).
REM Para cargar la BD usa cargarBaseDatos.bat antes de ejecutar este.
REM ===================================================================
title API Alumnos - http://localhost:50555

cd /d "%~dp0..\.."

echo ==========================================================
echo   EJECUTANDO API ALUMNOS EN http://localhost:50555
echo ==========================================================
echo.
echo Si la BD no esta cargada, ejecuta primero cargarBaseDatos.bat
echo Para detenerla presiona Ctrl+C en esta ventana.
echo Despues verifica con verEstadoConexionBD.bat
echo.

dotnet run --urls http://localhost:50555

echo.
echo La API se detuvo.
echo.
pause