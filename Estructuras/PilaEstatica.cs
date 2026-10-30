using System;

namespace Unidad3.Estructuras;

// Pila LIFO: el último elemento insertado es el primero que sale.
public sealed class PilaEstatica : IEstructuraEstatica
{
    private readonly int[] datos;
    private int top = -1; // -1 representa una pila vacía.
    public int Top => top;
    public int Capacidad => datos.Length;
    public int Cantidad => top + 1;
    public string Indices => $"top = {top}";

    public PilaEstatica(int capacidad)
    {
        if (capacidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidad));
        datos = new int[capacidad]; // Nunca se redimensiona.
    }

    public bool IsEmpty() => top == -1;
    public bool IsFull() => top == Capacidad - 1;

    public void Push(int valor)
    {
        // Comprobar antes de modificar evita corromper el estado si está llena.
        if (IsFull()) throw new InvalidOperationException("Pila llena.");
        top++;
        datos[top] = valor;
    }

    public int Pop()
    {
        if (IsEmpty()) throw new InvalidOperationException("Pila vacía.");
        int valor = datos[top];
        datos[top] = 0; // Borrado visual; el cero también es un dato válido.
        top--;
        return valor;
    }

    public int Peek()
    {
        if (IsEmpty()) throw new InvalidOperationException("Pila vacía.");
        return datos[top]; // Consultar no modifica top.
    }

    public void Limpiar()
    {
        for (int i = 0; i < Capacidad; i++) datos[i] = 0;
        top = -1;
    }

    // Adaptadores usados por la interfaz común; conservan la API Push y Pop.
    public void Insertar(int valor) => Push(valor);
    public int Retirar() => Pop();
    public bool Ocupada(int indice)
    {
        ValidarIndice(indice);
        return indice <= top;
    }
    public int ValorEn(int indice) { ValidarIndice(indice); return datos[indice]; }
    public string MarcaEn(int indice) => Ocupada(indice) && indice == top ? "top" : "";
    private void ValidarIndice(int indice)
    {
        if (indice < 0 || indice >= Capacidad)
            throw new ArgumentOutOfRangeException(nameof(indice));
    }
}
