using System.Collections.Generic;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempElements - kolekcja elementów tymczasowych
    public interface ITempElements : System.IDisposable
    {
        //Właściwość Elements - kolekcja elementów
        IReadOnlyCollection<ITempElement> Elements { get; }

        //Metoda AddElement - dodaje element do kolekcji
        T AddElement<T>() where T : ITempElement, new();

        //Metoda MoveElementTo - przenosi element do nowej lokalizacji na dysku
        void MoveElementTo<T>(T element, string nowaSciezka) where T : ITempElement;

        //Metoda DeleteElement - usuwa element z kolekcji i systemu plików
        void DeleteElement<T>(T element) where T : ITempElement;

        //Metoda RemoveDestroyed - usuwa z kolekcji elementy, które zostały zniszczone
        void RemoveDestroyed();
    }
}