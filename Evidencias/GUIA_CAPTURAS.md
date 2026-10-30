# Capturas reales pendientes en Windows

No se incluyen imágenes simuladas como evidencia. Ejecutar la aplicación y usar
**Guardar captura PNG** en cada estado. Guardar también el registro TXT. Estos resultados
son los esperados; deben verificarse al realizar las capturas.

| Archivo sugerido | Pasos | Resultado esperado |
| --- | --- | --- |
| 01_pila.png | Pestaña Pila → Cargar demostración | 10 y 20 ocupados; top=1; el registro muestra salida 30 |
| 02_pila_llena.png | Limpiar; insertar 1,2,3,4,5; intentar 6 | Rechazo; top=4; cantidad=5; isFull=True |
| 03_cola_simple.png | Pestaña Cola simple → Cargar demostración | front=2, rear=4, cantidad=3; insertar 60 se rechaza |
| 04_cola_circular.png | Pestaña Cola circular → Cargar demostración | Arreglo [60,70,30,40,50]; front=2, rear=1; cantidad=5 |
| 05_fifo_circular.png | Después de la anterior, retirar cinco veces | Salen 30,40,50,60,70; front=0, rear=-1; vacía |
| 06_vacia.png | Limpiar y retirar; después pulsar Peek | Mensajes de vacío; estructura permanece vacía |
| 07_entrada_invalida.png | Escribir abc e intentar insertar | Mensaje de entrada inválida; estado preservado |

Comprobar también un cero y un entero negativo: deben aparecer como datos ocupados.
Redimensionar la ventana y revisar que tabla, mensajes y botones sigan visibles.
La captura guarda el formulario; si el sistema presenta un control incompleto en la
imagen, usar la herramienta de recortes de Windows y capturar la ventana completa.

Registrar la versión de Windows y de .NET, fecha, resultado observado y cualquier
diferencia respecto de lo esperado. Agregar las imágenes al repositorio y al reporte
antes de entregar. La salida `pruebas_reales.txt` corresponde a la ejecución automatizada
de la lógica en Linux, no a una validación visual de Windows Forms.
