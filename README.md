# API de Gestión de Notas de Alumnos

## 📋 Descripción
API REST desarrollada en C# con .NET 10 para gestionar alumnos, asignaturas y notas.

## 🗄️ Setup de Base de Datos

### Opción 1: Crear la BD desde SQL Server Management Studio (Recomendado)

1. Abre **SQL Server Management Studio**
2. Conéctate a tu servidor SQL Server (ej: `(LocalDB)\mssqllocaldb`)
3. Abre el archivo `setup.sql` (Archivo → Abrir → Archivo)
4. Ejecuta el script (F5 o Click en "Ejecutar")
5. La base de datos `baseDatosAlumnos` se creará automáticamente

### Opción 2: Ejecutar desde línea de comandos

```powershell
sqlcmd -S (LocalDB)\mssqllocaldb -i setup.sql
```

## ▶️ Ejecutar la aplicación

```powershell
cd notasAlumnoAsignatura\AlumnosApi
dotnet run
```

La API estará disponible en: `https://localhost:5001`

## 📚 Documentación Swagger

Una vez ejecutando, abre en el navegador:
```
https://localhost:5001/swagger/index.html
```

## 🔌 Endpoints disponibles

### Alumnos
- **GET** `/api/alumnos` - Listar todos los alumnos
- **POST** `/api/alumnos` - Crear un nuevo alumno

### Asignaturas
- **GET** `/api/asignaturas` - Listar todas las asignaturas
- **POST** `/api/asignaturas` - Crear una nueva asignatura

### Notas
- **GET** `/api/notas/listado` - Listar notas con formato especial
- **POST** `/api/notas` - Ingresar una nueva nota
- **PUT** `/api/notas` - Editar una nota existente

## 📊 Estructura de Base de Datos

### Tabla ALUMNOS
- `COD_ALUMNO` (int) - Código único del alumno (PK)
- `NOMBRES` (varchar) - Nombres del alumno
- `APELLIDOS` (varchar) - Apellidos del alumno

### Tabla ASIGNATURAS
- `ID_ASIGNATURA` (int) - ID único (PK, Auto-increment)
- `NOM_ASIGNATURA` (varchar) - Nombre de la asignatura

### Tabla NOTAS
- `ID_NOTA` (int) - ID única (PK, Auto-increment)
- `NOTA` (decimal) - Valor de la nota (1.0 a 7.0)
- `N_NOTA` (int) - Número de nota (1, 2 o 3)
- `COD_ALUMNO` (int) - FK a ALUMNOS
- `ID_ASIGNATURA` (int) - FK a ASIGNATURAS

## 📝 Validaciones

- Nota mínima: **1.0**
- Nota máxima: **7.0**
- Decimales: Solo **1 decimal** (ej: 6.5)
- Número de nota: **1, 2 o 3**
- Promedio: Se calcula solo si están las **3 notas ingresadas**

## 🛠️ Tecnología

- **.NET 10**
- **Entity Framework Core 10**
- **SQL Server / LocalDB**
- **Swagger / OpenAPI**
- **ASP.NET Core**
