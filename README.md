# Tarea 3.1 · Pilas y colas estáticas

**Alumno:** Jesús Armando Zendejo Altamirano  
**Materia:** Estructuras de Datos · Unidad 3  
**Docente:** Ruben Riojas

Aplicación de Windows Forms en C# con tres pestañas: pila, cola simple y cola circular.
Cada estructura guarda sus datos en un `int[]` de capacidad fija y contiene comentarios en español.
No se utilizan `Stack<T>`, `Queue<T>`, `List<T>`, LINQ ni otras colecciones para implementar las estructuras.

## Abrir y ejecutar

Requisitos: Windows y SDK de .NET 8. En Visual Studio, instalar la carga de trabajo
**Desarrollo de escritorio de .NET**, abrir `Unidad3_Estaticas.sln` y seleccionar
**Aplicacion** como proyecto de inicio. Ejecutar con F5. El formulario se construye
desde código en `FormPrincipal.cs`; no requiere un archivo de diseñador.

Desde una terminal situada en la carpeta de la solución:

```powershell
dotnet run --project Aplicacion/Aplicacion.csproj -c Release
```

La interfaz usa una capacidad fija de cinco enteros por pestaña. Los constructores
aceptan otras capacidades positivas para las pruebas; ninguna estructura cambia
el tamaño de su arreglo durante su vida.

1. Escribir un entero y pulsar Push o Enqueue.
2. Retirar con Pop o Dequeue y consultar con Peek.
3. Revisar los índices, la cantidad, isEmpty, isFull y las posiciones físicas.
4. Pulsar **Cargar demostración** para reiniciar únicamente la pestaña activa y ejecutar un ejemplo.
5. Guardar una captura PNG y un registro TXT mediante los botones inferiores.

## Estructuras e índices

| Clase | Estado vacío | Lleno para insertar | Regla principal |
| --- | --- | --- | --- |
| PilaEstatica | top = -1 | top = capacidad - 1 | LIFO; Push incrementa top, Pop lo reduce |
| ColaSimple | front = 0, rear = -1 | rear = capacidad - 1 | FIFO; no desplaza ni reutiliza huecos hasta vaciarse |
| ColaCircular | cantidad = 0 | cantidad = capacidad | FIFO; avances con módulo y contador |

En ambas colas, `rear` es el índice del último elemento, no el siguiente espacio libre.
En la cola simple, `IsFull()` significa **sin espacio al final para insertar**, aunque
la cantidad sea menor que la capacidad. Al retirar el último elemento se normalizan
los índices. La cola circular utiliza las cinco posiciones, sin reservar una celda.
El cero y los negativos son datos válidos: la ocupación se determina por índices y cantidad.

Las API de la pila son `Push`, `Pop`, `Peek`, `IsEmpty`, `IsFull`; las colas ofrecen
`Enqueue`, `Dequeue`, `Peek`, `IsEmpty`, `IsFull`. C# usa PascalCase para estos métodos.
`IEstructuraEstatica` añade adaptadores `Insertar` y `Retirar` para reutilizar la vista.

Aquí “estática” significa capacidad fija, no memoria de pila de ejecución ni la palabra
clave `static`: los arreglos de C# son objetos administrados. Las colecciones de controles
de Windows Forms solo sirven para la presentación y no guardan los elementos de las estructuras.

## Pruebas automatizadas

```powershell
dotnet run --project Pruebas/Pruebas.csproj -c Release
```

Se ejecutaron **214629 verificaciones** con .NET 8.0.33 en Ubuntu 24.04.3 LTS.
Se probaron 2000 operaciones por combinación de estructura y capacidad (1 a 5):
30000 pasos en total, con semillas reproducibles y un modelo independiente basado
en un arreglo que conserva el orden lógico. Se verificaron resultados, cantidad,
estados lleno/vacío, celdas ocupadas y consultas sin modificación, incluso tras rechazos.
También se comprobaron capacidades inválidas, índices fuera de rango y secuencias conocidas.
El número de verificaciones cuenta afirmaciones, no casos manuales independientes.

`Evidencias/pruebas_reales.txt` conserva la salida obtenida. Las tres unidades de código
se compilaron con Roslyn del SDK 8.0.427; Windows Forms se compiló contra las referencias
Microsoft.WindowsDesktop.App.Ref 8.0.33. **La interfaz no se ejecutó en Windows en este entorno.**
Su ejecución visual, los cuadros de guardado y las capturas reales están pendientes de
comprobar en Windows siguiendo `Evidencias/GUIA_CAPTURAS.md`.

## Generar un ejecutable en Windows

```powershell
dotnet publish Aplicacion/Aplicacion.csproj -c Release -r win-x64 --self-contained true -o Publicacion
```

Abrir `Publicacion/Aplicacion.exe` y conservar toda la carpeta de publicación.
Ese comando descarga los componentes requeridos si no están instalados. No se incluye
un ejecutable publicado en este paquete; se entrega el proyecto fuente compilado y comprobado
en los términos descritos arriba.

## Complejidad

Insertar, retirar, Peek, IsEmpty e IsFull: O(1). Limpiar: O(C), donde C es la capacidad.
El almacenamiento de cada estructura es O(C), más índices O(1). La actualización de la
tabla gráfica recorre C celdas; ese costo no cambia la complejidad del algoritmo interno.

## Entrega y video

**Repositorio de GitHub:** pendiente de publicación.  
**Video público de YouTube:** pendiente de grabación y publicación.

Crear el repositorio y subir el contenido de esta carpeta, incluidos los proyectos,
README y evidencias. No subir `bin`, `obj` ni archivos de Visual Studio. Incorporar las
capturas reales después de probar la aplicación. El reporte adjunto incluye explicación,
pruebas de escritorio y guion; agregar los enlaces definitivos antes de entregar.
En el video mantener la cámara en la esquina inferior derecha y publicar en el mismo
canal utilizado para las otras tareas.

## Organización

- `Estructuras`: las tres implementaciones y el contrato para la vista.
- `Aplicacion`: formulario y punto de entrada de Windows Forms.
- `Pruebas`: programa de pruebas independiente, ejecutable sin Windows Forms.
- `Evidencias`: salida real y guía de capturas manuales.

## Referencias

- Microsoft. Arrays. https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays
- Microsoft. Windows Forms Overview. https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/
