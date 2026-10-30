using System;
using Unidad3.Estructuras;

namespace Unidad3.Pruebas;

internal static class Program
{
    private static int verificaciones;

    private static void Comprobar(bool condicion, string mensaje)
    {
        verificaciones++;
        if (!condicion) throw new Exception("Falló: " + mensaje);
    }

    // Verificar el tipo exacto evita aceptar errores ajenos al caso probado.
    private static void Esperar<T>(Action accion) where T : Exception
    {
        try { accion(); }
        catch (T) { verificaciones++; return; }
        throw new Exception("No se produjo " + typeof(T).Name);
    }

    private static IEstructuraEstatica Crear(int tipo, int capacidad) => tipo switch
    {
        0 => new PilaEstatica(capacidad),
        1 => new ColaSimple(capacidad),
        _ => new ColaCircular(capacidad)
    };

    private static void Main()
    {
        Console.WriteLine("TAREA 3.1 | Pruebas reales de las estructuras en C#");
        Console.WriteLine("Entorno: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
        Console.WriteLine("Sistema: " + System.Runtime.InteropServices.RuntimeInformation.OSDescription);
        for (int tipo = 0; tipo < 3; tipo++)
        {
            Esperar<ArgumentOutOfRangeException>(() => Crear(tipo, 0));
            Esperar<ArgumentOutOfRangeException>(() => Crear(tipo, -1));
            // Capacidad uno, vueltas repetidas, cero y negativos quedan incluidos.
            for (int capacidad = 1; capacidad <= 5; capacidad++)
                ProbarSecuencia(tipo, capacidad);
            Console.WriteLine("CORRECTO: " + Crear(tipo, 1).GetType().Name);
        }
        Demostracion();
        Console.WriteLine($"TOTAL: {verificaciones} verificaciones superadas.");
        Console.WriteLine("No se ejecutó Windows Forms en esta prueba de consola.");
    }

    private static void ProbarSecuencia(int tipo, int capacidad)
    {
        IEstructuraEstatica estructura = Crear(tipo, capacidad);
        int[] modelo = new int[capacidad]; // Oráculo independiente: orden lógico.
        int cantidad = 0;
        int insercionesDesdeVacio = 0;
        Random azar = new Random(3100 + tipo * 10 + capacidad);
        for (int paso = 0; paso < 2000; paso++)
        {
            // El modelo no calcula índices circulares: conserva el orden con desplazamientos.
            int operacion = azar.Next(10);
            bool llena = tipo == 1 ? insercionesDesdeVacio == capacidad : cantidad == capacidad;
            if (operacion < 5)
            {
                int valor = paso % 7 == 0 ? 0 : azar.Next(-100, 101);
                if (llena) Esperar<InvalidOperationException>(() => estructura.Insertar(valor));
                else
                {
                    estructura.Insertar(valor);
                    modelo[cantidad++] = valor;
                    insercionesDesdeVacio++;
                }
            }
            else if (operacion < 8)
            {
                if (cantidad == 0) Esperar<InvalidOperationException>(() => estructura.Retirar());
                else
                {
                    int esperado = tipo == 0 ? modelo[cantidad - 1] : modelo[0];
                    Comprobar(estructura.Retirar() == esperado, "orden LIFO o FIFO");
                    if (tipo != 0)
                        for (int i = 1; i < cantidad; i++) modelo[i - 1] = modelo[i];
                    cantidad--;
                    if (cantidad == 0) insercionesDesdeVacio = 0;
                }
            }
            else if (operacion == 9)
            {
                estructura.Limpiar(); cantidad = 0; insercionesDesdeVacio = 0;
            }

            // Se compara el estado después de operaciones válidas y rechazadas.
            Comprobar(estructura.Capacidad == capacidad, "capacidad fija");
            Comprobar(estructura.Cantidad == cantidad, "cantidad");
            Comprobar(estructura.IsEmpty() == (cantidad == 0), "vacío");
            llena = tipo == 1 ? insercionesDesdeVacio == capacidad : cantidad == capacidad;
            Comprobar(estructura.IsFull() == llena, "lleno para inserción");
            int ocupadas = 0;
            for (int i = 0; i < capacidad; i++)
                if (estructura.Ocupada(i)) ocupadas++;
            Comprobar(ocupadas == cantidad, "celdas ocupadas");
            if (cantidad == 0) Esperar<InvalidOperationException>(() => estructura.Peek());
            else
            {
                int esperado = tipo == 0 ? modelo[cantidad - 1] : modelo[0];
                string indicesAntes = estructura.Indices;
                Comprobar(estructura.Peek() == esperado, "Peek");
                Comprobar(estructura.Indices == indicesAntes && estructura.Cantidad == cantidad,
                    "Peek no modifica estado");
            }
        }
        Esperar<ArgumentOutOfRangeException>(() => estructura.ValorEn(-1));
        Esperar<ArgumentOutOfRangeException>(() => estructura.Ocupada(capacidad));
    }

    private static void Demostracion()
    {
        Console.WriteLine("DEMOSTRACIÓN | capacidad 5");
        var pila = new PilaEstatica(5);
        pila.Push(10); pila.Push(20); pila.Push(30);
        Comprobar(pila.Pop() == 30 && pila.Peek() == 20 && pila.Top == 1, "pila conocida");
        Console.WriteLine("Pila: Push 10,20,30; Pop=30; Peek=20; top=1.");
        var simple = new ColaSimple(5);
        var circular = new ColaCircular(5);
        for (int i = 1; i <= 5; i++) { simple.Enqueue(i * 10); circular.Enqueue(i * 10); }
        Comprobar(simple.Dequeue() == 10 && simple.Dequeue() == 20, "simple FIFO");
        Comprobar(circular.Dequeue() == 10 && circular.Dequeue() == 20, "circular FIFO");
        Esperar<InvalidOperationException>(() => simple.Enqueue(60));
        Comprobar(simple.Front == 2 && simple.Rear == 4 && simple.Cantidad == 3,
            "simple conserva estado después del rechazo");
        Console.WriteLine("Simple: quedan 30,40,50; front=2; rear=4; Enqueue 60 rechazado.");
        circular.Enqueue(60); circular.Enqueue(70);
        Comprobar(circular.Front == 2 && circular.Rear == 1 && circular.IsFull(), "vuelta circular");
        Console.WriteLine("Circular: arreglo [60,70,30,40,50]; front=2; rear=1; llena=True.");
        int[] esperado = { 30, 40, 50, 60, 70 };
        for (int i = 0; i < esperado.Length; i++)
            Comprobar(circular.Dequeue() == esperado[i], "FIFO después de la vuelta");
        Comprobar(circular.IsEmpty() && circular.Front == 0 && circular.Rear == -1,
            "normalización al vaciar");
        Console.WriteLine("Circular: salida 30,40,50,60,70; vacía=True; front=0; rear=-1.");
    }
}
