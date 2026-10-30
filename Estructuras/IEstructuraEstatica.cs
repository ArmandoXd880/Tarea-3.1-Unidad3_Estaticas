using System;

namespace Unidad3.Estructuras;

// Contrato común para conectar la interfaz y las pruebas con las estructuras.
// Los elementos se almacenan únicamente en arreglos int[] de longitud fija.
public interface IEstructuraEstatica
{
    int Capacidad { get; }
    int Cantidad { get; }
    string Indices { get; }
    bool IsEmpty();
    bool IsFull();
    void Insertar(int valor);
    int Retirar();
    int Peek();
    void Limpiar();
    bool Ocupada(int indice);
    int ValorEn(int indice);
    string MarcaEn(int indice);
}
