using System;
using System.IO;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Klasa TempDir pozwala zarządzać katalogiem tymczasowym na dysku
    public class TempDir : ITempDir
    {
        //Pole DirectoryInfo przechowuje informacje o katalogu tymczasowym
        public DirectoryInfo infoKatalogu { get; }

        //Właściwość Path zwraca pełną ścieżkę do katalogu tymczasowego
        public string Path => infoKatalogu.FullName;

        //Właściwość IsDestroyed informuje, czy katalog został już usunięty z dysku
        public bool IsDestroyed { get; private set; }

        //Flaga prywatna sprawdzająca czy zasób został już zwolniony
        private bool _zwolniony;

        //Konstruktor domyślny tworzy nowy katalog tymczasowy w domyślnej lokalizacji
        public TempDir()
        {
            try
            {
                //Tworzymy ścieżkę do nowego katalogu tymczasowego i nadajemy mu unikalną nazwę
                var sciezka = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
                //Tworzymy katalog na dysku
                infoKatalogu = Directory.CreateDirectory(sciezka);
                IsDestroyed = false;
                _zwolniony = false;
            }
            //Jeżeli wystąpił błąd podczas tworzenia katalogu, zgłaszamy wyjątek
            catch (IOException ex)
            {
                throw new IOException("Błąd podczas tworzenia katalogu tymczasowego.", ex);
            }
        }

        //Konstruktor z możliwością podania ścieżki do katalogu
        public TempDir(string sciezka)
        {
            //Jeśli ścieżka jest pusta, zgłaszamy wyjątek
            if (string.IsNullOrWhiteSpace(sciezka))
                throw new ArgumentException("Podana ścieżka nie może być pusta.", nameof(sciezka));
            try
            {
                //Tworzymy katalog na podanej ścieżce
                infoKatalogu = Directory.CreateDirectory(sciezka);
                IsDestroyed = false;
                _zwolniony = false;
            }
            //Jeżeli wystąpił błąd podczas tworzenia katalogu, zgłaszamy wyjątek
            catch (IOException ex)
            {
                throw new IOException("Nie udało się utworzyć katalogu tymczasowego pod wskazaną ścieżką.", ex);
            }
        }

        //Metoda Delete pozwala usunąć katalog z dysku oraz ustawić flagę IsDestroyed
        public void Delete()
        {
            //Sprawdzamy czy katalog nie został już usunięty
            EnsureNotDisposed();
            if (!IsDestroyed)
            {
                //Odświeżamy informacje o katalogu
                infoKatalogu.Refresh();
                //Jeśli katalog istnieje, usuwamy go razem z całą zawartością
                if (infoKatalogu.Exists)
                    infoKatalogu.Delete(true);
                //Ustawiamy flagę IsDestroyed na true
                IsDestroyed = true;
            }
        }

        //Metoda pomocnicza sprawdzająca czy katalog nie został już zwolniony, zgłasza wyjątek w razie potrzeby
        private void EnsureNotDisposed()
        {
            if (_zwolniony)
                throw new ObjectDisposedException(nameof(TempDir));
        }

        //Metoda Dispose odpowiedzialna za poprawne zwolnienie zasobów
        protected virtual void Dispose(bool disposing)
        {
            if (!_zwolniony)
            {
                //Usuwamy katalog
                Delete();
                _zwolniony = true;
            }
        }

        //Finalizer, który wywołuje Dispose w przypadku braku jawnego zwolnienia zasobów
        ~TempDir()
        {
            Dispose(false);
        }

        //Publiczna metoda Dispose - jawne zwolnienie zasobów
        public void Dispose()
        {
            //Wywołujemy Dispose z parametrem true, co oznacza jawne zwolnienie zasobów
            Dispose(true);

            //Informujemy system, że nie trzeba wywoływać finalizera
            GC.SuppressFinalize(this);
        }

        //Implementacja wymaganej właściwości DirectoryInfo z interfejsu ITempDir
        public DirectoryInfo DirectoryInfo => infoKatalogu;
    }
}