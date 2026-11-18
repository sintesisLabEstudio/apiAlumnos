@echo off
REM BORRA la BD ALUMNOS del motor SQL (DROP DATABASE): se pierden los datos
REM y se eliminan los archivos .mdf y .ldf.
REM Pasa opciones, por ejemplo: borrarBaseDatosAlumnos.bat -Force -StopLocalDB
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\detalleComandosBat\borrarBaseDatosAlumnos.ps1" %*
echo.
pause
