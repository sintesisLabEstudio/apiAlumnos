@echo off
setlocal
title Respaldar base de datos ALUMNOS

echo ==========================================================
echo   RESPALDAR BASE DE DATOS ALUMNOS
echo ==========================================================
echo.
echo Copiara los archivos .mdf y .ldf originales a la carpeta BD,
echo sobrescribiendo las copias anteriores.
echo.

call "%~dp0copiarBaseDatosAlumnosABD.bat"
exit /b %ERRORLEVEL%