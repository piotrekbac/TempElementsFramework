using System;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempElement - bazowy dla wszystkich elementów tymczasowych
    public interface ITempElement : IDisposable
    {
        //Właściwość Path - ścieżka do zasobu tymczasowego (plik, katalog)
        string Path { get; }

        //Właściwość IsDestroyed - informacja, czy zasób został już zniszczony
        bool IsDestroyed { get; }

        //Metoda Delete - usuwa zasób powiązany z obiektem
        void Delete();
    }
}