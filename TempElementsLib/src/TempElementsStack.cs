using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Klasa TempElementsStack zarządza historią elementów tymczasowych używając stosu (LIFO).
    //Umożliwia dodawanie nowych elementów, wycofywanie ostatnich operacji (undo), przenoszenie i usuwanie elementów.
    //Przydaje się np. do implementacji historii zmian (np. w edytorze plików graficznych).
    public class TempElementsStack : ITempElementsStack
    {
        //Stos do przechowywania elementów tymczasowych (LIFO)
        private readonly Stack<ITempElement> _elementy = new Stack<ITempElement>();

        //Property Elements - tylko do odczytu, umożliwia przeglądanie historii (najpierw ostatnio dodane)
        public IReadOnlyCollection<ITempElement> Elements => new ReadOnlyCollection<ITempElement>(new List<ITempElement>(_elementy));

        //Dodaj nowy element typu ITempElement (TempFile, TempDir, TempTxtFile, ...) na wierzch stosu
        public T AddElement<T>() where T : ITempElement, new()
        {
            //Tworzymy nowy element i umieszczamy na szczycie stosu
            T elem = new T();
            _elementy.Push(elem);
            return elem;
        }

        //Przenosi element do nowej lokalizacji (usuwa element ze stosu i przenosi jego zasób)
        public void MoveElementTo<T>(T element, string nowaSciezka) where T : ITempElement
        {
            if (!_elementy.Contains(element))
                throw new InvalidOperationException("Element nie istnieje na stosie!");

            element.Dispose();

            if (element is ITempFile plik)
            {
                System.IO.File.Move(plik.Path, nowaSciezka);
            }
            else if (element is ITempDir katalog)
            {
                System.IO.Directory.Move(katalog.Path, nowaSciezka);
            }

            //Usuwamy element ze stosu (z dowolnego miejsca)
            var tymczasowy = new Stack<ITempElement>();
            bool usunieto = false;
            while (_elementy.Count > 0)
            {
                var e = _elementy.Pop();
                if (!usunieto && ReferenceEquals(e, element))
                {
                    usunieto = true;
                    continue; // pomijamy ten element
                }
                tymczasowy.Push(e);
            }
            while (tymczasowy.Count > 0)
                _elementy.Push(tymczasowy.Pop());
        }

        //Usuwa element ze stosu oraz zwalnia jego zasób
        public void DeleteElement<T>(T element) where T : ITempElement
        {
            if (!_elementy.Contains(element))
                return;

            element.Delete();

            //Usuwamy element ze stosu (z dowolnego miejsca)
            var tymczasowy = new Stack<ITempElement>();
            bool usunieto = false;
            while (_elementy.Count > 0)
            {
                var e = _elementy.Pop();
                if (!usunieto && ReferenceEquals(e, element))
                {
                    usunieto = true;
                    continue;
                }
                tymczasowy.Push(e);
            }
            while (tymczasowy.Count > 0)
                _elementy.Push(tymczasowy.Pop());
        }

        //Usuwa wszystkie elementy, które zostały już zniszczone (IsDestroyed == true)
        public void RemoveDestroyed()
        {
            var nowyStos = new Stack<ITempElement>();
            foreach (var el in _elementy)
            {
                if (!el.IsDestroyed)
                    nowyStos.Push(el);
            }
            _elementy.Clear();
            foreach (var el in nowyStos)
                _elementy.Push(el);
        }

        //Usuwa (wycofuje) ostatnio dodany element (undo)
        public ITempElement Pop()
        {
            if (_elementy.Count == 0)
                throw new InvalidOperationException("Stos jest pusty!");

            var ostatni = _elementy.Pop();
            ostatni.Dispose();
            return ostatni;
        }

        //Dispose - zwalnia wszystkie zasoby i czyści stos
        public void Dispose()
        {
            while (_elementy.Count > 0)
            {
                var elem = _elementy.Pop();
                elem.Dispose();
            }
        }
    }
}