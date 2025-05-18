using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Klasa TempElementsList pozwala na zarządzanie kolekcją elementów tymczasowych
    public class TempElementsList : ITempElements
    {
        //Lista przechowująca elementy tymczasowe (pliki i katalogi)
        private readonly List<ITempElement> _elementy = new List<ITempElement>();

        //Property Elements tylko do odczytu, pozwala na przeglądanie kolekcji
        public IReadOnlyCollection<ITempElement> Elements => new ReadOnlyCollection<ITempElement>(_elementy);

        //Dodaje nowy element typu ITempElement (może być TempFile, TempDir itp.)
        public T AddElement<T>() where T : ITempElement, new()
        {
            //Tworzymy nowy element i dodajemy do kolekcji
            T elem = new T();
            _elementy.Add(elem);
            //Zwracamy referencję do utworzonego elementu
            return elem;
        }

        //Przenosi element do innej lokalizacji na dysku (dla pliku lub katalogu)
        public void MoveElementTo<T>(T element, string nowaSciezka) where T : ITempElement
        {
            //Jeśli przenosimy plik
            if (element is ITempFile plik)
            {
                //Zwalniamy zasoby pliku
                element.Dispose();
                //Przenosimy plik na nową ścieżkę
                System.IO.File.Move(plik.Path, nowaSciezka);
            }
            //Jeśli przenosimy katalog
            else if (element is ITempDir katalog)
            {
                //Zwalniamy zasoby katalogu
                element.Dispose();
                //Przenosimy katalog na nową ścieżkę
                System.IO.Directory.Move(katalog.Path, nowaSciezka);
            }
        }

        //Usuwa element z kolekcji oraz z systemu plików
        public void DeleteElement<T>(T element) where T : ITempElement
        {
            //Usuwamy zasób związany z elementem i referencję z kolekcji
            element.Delete();
            _elementy.Remove(element);
        }

        //Usuwa z kolekcji elementy już zniszczone
        public void RemoveDestroyed()
        {
            //Usuwamy z listy elementy, których flaga IsDestroyed jest ustawiona na true
            _elementy.RemoveAll(e => e.IsDestroyed);
        }

        //Dispose - usuwa wszystkie elementy i czyści kolekcję
        public void Dispose()
        {
            //Zwalniamy zasoby każdego elementu
            foreach (var el in _elementy)
                el.Dispose();
            //Czyścimy kolekcję
            _elementy.Clear();
        }
    }
}