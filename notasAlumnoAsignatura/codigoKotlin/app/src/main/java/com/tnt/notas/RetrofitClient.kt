package com.tnt.notas
import android.os.Build
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.ResponseBody
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.util.concurrent.TimeUnit
object RetrofitClient {
    private val host: String = if (esEmulador()) "10.0.2.2" else "127.0.0.1"
    val BASE_URL = "http://$host:50555/"
    private fun esEmulador(): Boolean {
        return Build.FINGERPRINT.startsWith("generic")
                || Build.FINGERPRINT.startsWith("unknown")
                || Build.MODEL.contains("google_sdk")
                || Build.MODEL.contains("Emulator")
                || Build.MODEL.contains("Android SDK built for x86")
                || Build.MANUFACTURER.contains("Genymotion")
                || (Build.BRAND.startsWith("generic") && Build.DEVICE.startsWith("generic"))
                || "google_sdk" == Build.PRODUCT
    }
    private val cliente: OkHttpClient = OkHttpClient.Builder()
        .connectTimeout(8, TimeUnit.SECONDS)
        .readTimeout(8, TimeUnit.SECONDS)
        .build()
    val api: ApiService = Retrofit.Builder()
        .baseUrl(BASE_URL)
        .client(cliente)
        .addConverterFactory(GsonConverterFactory.create())
        .build()
        .create(ApiService::class.java)
    suspend fun guardar(llamada: suspend () -> Response<ResponseBody>): String {
        return withContext(Dispatchers.IO) {
            try {
                val r = llamada()
                if (r.isSuccessful) {
                    "Guardado correctamente"
                } else {
                    val error = r.errorBody()?.string() ?: ""
                    if (error == "") "Error " + r.code() else error.trim('"')
                }
            } catch (e: Exception) {
                "No se pudo conectar con la API: " + e.message
            }
        }
    }
}
