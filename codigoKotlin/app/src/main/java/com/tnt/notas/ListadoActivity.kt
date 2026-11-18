package com.tnt.notas
import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
class ListadoActivity : AppCompatActivity() {
    lateinit var tvListado: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_listado)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        tvListado = findViewById(R.id.tvListado)
        findViewById<Button>(R.id.btnActualizar).setOnClickListener { cargar() }
        cargar()
    }
    fun cargar() {
        lifecycleScope.launch {
            tvListado.text = "Cargando..."
            try {
                val respuesta = withContext(Dispatchers.IO) { RetrofitClient.api.listado() }
                if (!respuesta.isSuccessful) {
                    tvListado.text = "Error " + respuesta.code()
                    return@launch
                }
                val filas = respuesta.body() ?: emptyList()
                if (filas.isEmpty()) {
                    tvListado.text = "Aun no hay notas ingresadas"
                    return@launch
                }
                var texto = ""
                for (f in filas) {
                    texto += "Alumno: " + f.codAlumno + " - " + f.nombreCompleto + "\n"
                    texto += "Asignatura: " + f.nomAsignatura + "\n"
                    if (f.nota1 != null) texto += "Nota 1: " + f.nota1 + "\n"
                    if (f.nota2 != null) texto += "Nota 2: " + f.nota2 + "\n"
                    if (f.nota3 != null) texto += "Nota 3: " + f.nota3 + "\n"
                    if (f.promedio != null) texto += "Promedio: " + f.promedio + "\n"
                    texto += "\n----------------------\n\n"
                }
                tvListado.text = texto
            } catch (e: Exception) {
                tvListado.text = "No se pudo conectar con la API: " + e.message
            }
        }
    }
}
