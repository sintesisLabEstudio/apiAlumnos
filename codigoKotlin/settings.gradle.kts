// Configuracion de repositorios para la resolucion de plugins.
pluginManagement {
    repositories {
        google()
        mavenCentral()
        gradlePluginPortal()
    }
}
// Configuracion de repositorios para dependencias del proyecto.
dependencyResolutionManagement {
    repositoriesMode.set(RepositoriesMode.FAIL_ON_PROJECT_REPOS)
    repositories {
        google()
        mavenCentral()
    }
}
// Nombre del proyecto y modulos incluidos.
rootProject.name = "codigoKotlin"
include(":app")