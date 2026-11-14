-- baseDatosAlumnos_creacion.sql
-- Crea la base de datos baseDatosAlumnos y las tablas ALUMNOS, ASIGNATURAS y NOTAS.
-- Los nombres de tablas y columnas coinciden con AlumnosContext.cs (tablas en plural).
-- Es re-ejecutable: no borra nada y solo crea lo que no existe.
-- No usa rutas de archivo fijas, por lo que funciona en cualquier equipo con SQL Server, Express o LocalDB.
USE [master]
GO
-- Crea la base de datos solo si no existe. SQL Server elige la carpeta de datos por defecto.
IF DB_ID(N'baseDatosAlumnos') IS NULL
    CREATE DATABASE [baseDatosAlumnos]
GO
USE [baseDatosAlumnos]
GO
-- Tabla ALUMNOS: COD_ALUMNO lo ingresa el usuario (no es autonumerico, igual que ValueGeneratedNever en el contexto).
IF OBJECT_ID(N'dbo.ALUMNOS', N'U') IS NULL
CREATE TABLE [dbo].[ALUMNOS](
    [COD_ALUMNO] [int] NOT NULL,
    [NOMBRES]    [varchar](50) NOT NULL,
    [APELLIDOS]  [varchar](50) NOT NULL,
    CONSTRAINT [PK_ALUMNOS] PRIMARY KEY CLUSTERED ([COD_ALUMNO] ASC)
)
GO
-- Tabla ASIGNATURAS: ID_ASIGNATURA es autonumerico y el nombre no se puede repetir (la API tambien lo valida).
IF OBJECT_ID(N'dbo.ASIGNATURAS', N'U') IS NULL
CREATE TABLE [dbo].[ASIGNATURAS](
    [ID_ASIGNATURA]  [int] IDENTITY(1,1) NOT NULL,
    [NOM_ASIGNATURA] [varchar](50) NOT NULL,
    CONSTRAINT [PK_ASIGNATURAS] PRIMARY KEY CLUSTERED ([ID_ASIGNATURA] ASC),
    CONSTRAINT [UQ_ASIGNATURAS_NOMBRE] UNIQUE ([NOM_ASIGNATURA])
)
GO
-- Tabla NOTAS: una fila por cada nota (hasta 3 por alumno y asignatura).
-- N_NOTA indica cual de las 3 notas es (1, 2 o 3). NOTA es decimal(2,1): un solo decimal.
IF OBJECT_ID(N'dbo.NOTAS', N'U') IS NULL
CREATE TABLE [dbo].[NOTAS](
    [ID_NOTA]        [int] IDENTITY(1,1) NOT NULL,
    [NOTA]           [decimal](2,1) NOT NULL,
    [N_NOTA]         [int] NOT NULL,
    [COD_ALUMNO]     [int] NOT NULL,
    [ID_ASIGNATURA]  [int] NOT NULL,
    CONSTRAINT [PK_NOTAS] PRIMARY KEY CLUSTERED ([ID_NOTA] ASC),
    -- Relaciones: toda nota pertenece a un alumno y a una asignatura existentes.
    CONSTRAINT [FK_NOTAS_ALUMNOS] FOREIGN KEY ([COD_ALUMNO]) REFERENCES [dbo].[ALUMNOS]([COD_ALUMNO]),
    CONSTRAINT [FK_NOTAS_ASIGNATURAS] FOREIGN KEY ([ID_ASIGNATURA]) REFERENCES [dbo].[ASIGNATURAS]([ID_ASIGNATURA]),
    -- Reglas del enunciado: N_NOTA entre 1 y 3, y nota entre 1.0 y 7.0.
    CONSTRAINT [CK_NOTAS_N_NOTA] CHECK ([N_NOTA] BETWEEN 1 AND 3),
    CONSTRAINT [CK_NOTAS_NOTA] CHECK ([NOTA] BETWEEN 1.0 AND 7.0),
    -- Impide repetir la misma nota (N_NOTA) para un mismo alumno y asignatura, incluso con peticiones simultaneas.
    CONSTRAINT [UQ_NOTAS_ALUMNO_ASIGNATURA_NNOTA] UNIQUE ([COD_ALUMNO], [ID_ASIGNATURA], [N_NOTA])
)
GO
-- DATOS DE EJEMPLO (opcional). Estan comentados para entregar la base vacia.
-- Para probar el listado, quitar los marcadores /* y */ y ejecutar este bloque.
/*
INSERT INTO [dbo].[ALUMNOS] ([COD_ALUMNO], [NOMBRES], [APELLIDOS]) VALUES (1001, 'Ana', 'Rojas');
INSERT INTO [dbo].[ALUMNOS] ([COD_ALUMNO], [NOMBRES], [APELLIDOS]) VALUES (1002, 'Luis', 'Soto');
INSERT INTO [dbo].[ASIGNATURAS] ([NOM_ASIGNATURA]) VALUES ('Taller de Nuevas Tecnologias');
INSERT INTO [dbo].[ASIGNATURAS] ([NOM_ASIGNATURA]) VALUES ('Base de Datos');
-- Ana con las 3 notas en Taller (promedio calculable) y Luis con 2 notas en Base de Datos (sin promedio).
INSERT INTO [dbo].[NOTAS] ([NOTA], [N_NOTA], [COD_ALUMNO], [ID_ASIGNATURA])
SELECT 6.0, 1, 1001, ID_ASIGNATURA FROM [dbo].[ASIGNATURAS] WHERE NOM_ASIGNATURA = 'Taller de Nuevas Tecnologias';
INSERT INTO [dbo].[NOTAS] ([NOTA], [N_NOTA], [COD_ALUMNO], [ID_ASIGNATURA])
SELECT 5.5, 2, 1001, ID_ASIGNATURA FROM [dbo].[ASIGNATURAS] WHERE NOM_ASIGNATURA = 'Taller de Nuevas Tecnologias';
INSERT INTO [dbo].[NOTAS] ([NOTA], [N_NOTA], [COD_ALUMNO], [ID_ASIGNATURA])
SELECT 6.5, 3, 1001, ID_ASIGNATURA FROM [dbo].[ASIGNATURAS] WHERE NOM_ASIGNATURA = 'Taller de Nuevas Tecnologias';
INSERT INTO [dbo].[NOTAS] ([NOTA], [N_NOTA], [COD_ALUMNO], [ID_ASIGNATURA])
SELECT 4.0, 1, 1002, ID_ASIGNATURA FROM [dbo].[ASIGNATURAS] WHERE NOM_ASIGNATURA = 'Base de Datos';
INSERT INTO [dbo].[NOTAS] ([NOTA], [N_NOTA], [COD_ALUMNO], [ID_ASIGNATURA])
SELECT 5.0, 2, 1002, ID_ASIGNATURA FROM [dbo].[ASIGNATURAS] WHERE NOM_ASIGNATURA = 'Base de Datos';
*/
GO
