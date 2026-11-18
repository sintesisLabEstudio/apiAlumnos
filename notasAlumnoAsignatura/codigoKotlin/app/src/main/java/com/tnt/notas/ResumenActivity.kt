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
class ResumenActivity : AppCompatActivity() {
    lateinit var tvResumen: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_resumen)
        findViewById<Button>(R.id.btnHome).setOnClickListener {
            val intent = Intent(this, MainActivity::class.java)
            intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP or Intent.FLAG_ACTIVITY_SINGLE_TOP)
            startActivity(intent)
            finish()
        }
        tvResumen = findViewById(R.id.tvResumen)
        findViewById<Button>(R.id.btnActualizar).setOnClickListener { cargar() }
        cargar()
    }
    fun cargar() {
        lifecycleScope.launch {
            tvResumen.text = "Cargando..."
            try {
                val respuesta = withContext(Dispatchers.IO) { RetrofitClient.api.listado() }
                if (!respuesta.isSuccessful) {
                    tvResumen.text = "Error " + respuesta.code()
                    return@launch
                }
                val filas = respuesta.body() ?: emptyList()
                if (filas.isEmpty()) {
                    tvResumen.text = "Aun no hay notas ingresadas"
                    return@launch
                }
                var texto = ""
                for (f in filas) {
                    texto += "COD_ALUMNO: " + (f.codAlumno?.toString() ?: "-") + "\n"
                    texto += "NOMBRE_COMPLETO: " + (f.nombreCompleto ?: "-") + "\n"
                    texto += "NOM_ASIGNATURA: " + (f.nomAsignatura ?: "-") + "\n"
                    if (f.nota1 != null) {
                        texto += "N_NOTA_1: 1\n"
                        texto += "NOTA_1: " + f.nota1 + "\n"
                    }
                    if (f.nota2 != null) {
                        texto += "N_NOTA2: 2\n"
                        texto += "NOTA_2: " + f.nota2 + "\n"
                    }
                    if (f.nota3 != null) {
                        texto += "N_NOTA_3: 3\n"
                        texto += "NOTA_3: " + f.nota3 + "\n"
                    }
                    if (f.promedio != null) {
                        texto += "PROMEDIO: " + f.promedio + "\n"
                    }
                    texto += "----------------------\n"
                }
                tvResumen.text = texto
            } catch (e: Exception) {
                tvResumen.text = "No se pudo conectar con la API: " + e.message
            }
        }
    }
}
