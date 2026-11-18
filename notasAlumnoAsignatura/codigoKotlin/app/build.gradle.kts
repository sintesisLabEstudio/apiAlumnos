// Plugins necesarios para compilar una aplicacion Android en Kotlin.
plugins {
    id("com.android.application")
    id("org.jetbrains.kotlin.android")
}
// Configuracion Android del modulo app.
android {
    // Paquete base y version de SDK de compilacion.
    namespace = "com.tnt.notas"
    compileSdk = 35
    // Parametros de la aplicacion (id, versiones y runner de pruebas).
    defaultConfig {
        applicationId = "com.tnt.notas"
        minSdk = 24
        targetSdk = 35
        versionCode = 1
        versionName = "1.0"
        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }
    // Tipos de compilacion (release sin minificacion en este caso).
    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }
    // Compatibilidad de compilacion con Java 17.
    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }
    // Version de JVM objetivo para Kotlin.
    kotlinOptions {
        jvmTarget = "17"
    }
}
// Dependencias del proyecto (AndroidX, Retrofit, Gson y corrutinas).
dependencies {
    implementation("androidx.core:core-ktx:1.15.0")
    implementation("androidx.appcompat:appcompat:1.7.0")
    implementation("com.google.android.material:material:1.12.0")
    implementation("androidx.constraintlayout:constraintlayout:2.2.0")
    implementation("androidx.lifecycle:lifecycle-runtime-ktx:2.8.7")
    // Librerias para consumo de API REST y manejo asincrono.
    implementation("com.squareup.retrofit2:retrofit:2.11.0")
    implementation("com.squareup.retrofit2:converter-gson:2.11.0")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.8.1")
}
