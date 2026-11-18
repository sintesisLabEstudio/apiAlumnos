# ===================================================================
# CREAR BASE DE DATOS ALUMNOS
# Crea la BD ALUMNOS ejecutando el script original del profesor
# (ALUMNOS.sql, en la carpeta Data, carpeta padre de comandos). No modifica el script.
# Si la BD ya existe, conserva sus datos.
# ===================================================================
$ErrorActionPreference = 'Stop'
$server = '(localdb)\MSSQLLocalDB'
$script = Join-Path $PSScriptRoot '..\ALUMNOS.sql'

Write-Host '1/3 Verificando LocalDB...'
$info = & sqllocaldb info MSSQLLocalDB 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host 'Creando instancia MSSQLLocalDB...'
    & sqllocaldb create MSSQLLocalDB
}
$state = (& sqllocaldb info MSSQLLocalDB) | Select-String 'State:\s*Running'
if (-not $state) {
    Write-Host 'Iniciando MSSQLLocalDB...'
    & sqllocaldb start MSSQLLocalDB
}

Write-Host '2/3 Creando la BD ALUMNOS si no existe...'
$existe = & sqlcmd -S $server -E -h -1 -W -Q "IF DB_ID('ALUMNOS') IS NULL SELECT 0 ELSE SELECT 1" | Where-Object { $_.Trim() -ne '' } | Select-Object -First 1
if ([int]$existe -eq 0) {
    & sqlcmd -S $server -E -i $script
    Write-Host 'BD ALUMNOS creada desde ALUMNOS.sql.'
} else {
    Write-Host 'La BD ALUMNOS ya existe; se conservan sus datos.'
}

Write-Host '3/3 Tablas de la BD ALUMNOS:'
& sqlcmd -S $server -E -d ALUMNOS -Q "SELECT name FROM sys.tables"
Write-Host 'Listo. Ahora puedes ejecutar: dotnet run --urls http://localhost:50555'
