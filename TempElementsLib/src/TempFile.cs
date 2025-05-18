using System;
using System.IO;
using System.Text;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Klasa TempFile odpowiada za obsługę pliku tymczasowego, umożliwia bezpieczne zarządzanie jego życiem
    public class TempFile : ITempFile
    {
        //Pole readonly typu FileStream, służy do operacji na pliku binarnym (odczyt/zapis)
        public readonly FileStream strumienPliku;

        //Pole readonly typu FileInfo zawierające dane o pliku
        public readonly FileInfo infoPliku;

        //Flaga prywatna, która pozwala sprawdzić czy zasób został już zwolniony przez użytkownika
        private bool _zamkniety;

        //Właściwość Path - zwraca pełną ścieżkę do utworzonego pliku tymczasowego
        public string Path => infoPliku.FullName;

        //Właściwość FileStream zapewniająca dostęp do strumienia pliku (sprawdza, czy obiekt nie został zniszczony)
        public FileStream FileStream
        {
            get
            {
                //Jeżeli plik jest usunięty, zgłaszamy błąd 
                EnsureNotDisposed();
                //Zwracamy strumień pliku 
                return strumienPliku;
            }
        }

        //Właściwość FileInfo - dostęp do informacji o pliku (sprawdza, czy obiekt nie został zniszczony)
        public FileInfo FileInfo
        {
            get
            {
                //Jeżeli plik jest usunięty, zgłaszamy błąd
                EnsureNotDisposed();

                //Zwracamy informacje o pliku
                return infoPliku;
            }
        }

        //Flaga publiczna informująca o tym, czy plik został już usunięty
        public bool IsDestroyed { get; private set; }

        //Konstruktor domyślny: tworzy plik tymczasowy w katalogu systemowym
        public TempFile()
        {
            try
            {
                //Tworzymy plik tymczasowy w katalogu systemowym i nadajemy mu unikalną nazwę
                var tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());

                //Tworzymy nowy plik na dysku i tworzymy do niego strumień w celu zapisu i odczytu
                strumienPliku = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

                //Tworzymy obiekt FileInfo, który zawiera informacje o pliku
                infoPliku = new FileInfo(tempFilePath);

                //Ustawiamy flagę IsDestroyed na false, ponieważ plik został właśnie utworzony - NIE JEST ZNISZCZONY
                IsDestroyed = false;

                //Ustawiamy flagę _zamkniety na false, ponieważ obiekt nie jest zamknięty 
                _zamkniety = false;
            }
            //Jeżeli wystąpił wyjątek podczas tworzenia pliku, zgłaszamy błąd
            catch (IOException ioErr)
            {
                //Wyjątek jeśli nie uda się utworzyć pliku tymczasowego
                throw new IOException("Nie można utworzyć pliku tymczasowego.", ioErr);
            }
        }

        //Definiujemy konstruktor z możliwością wskazania ścieżki
        public TempFile(string fullFilePath)
        {
            //Jeżeli podana ścieżka jest pusta, zgłaszamy wyjątek ArgumentException 
            if (string.IsNullOrWhiteSpace(fullFilePath))
                throw new ArgumentException("Podana ścieżka do pliku nie może być pusta.", nameof(fullFilePath));

            try
            {
                //Tworzymy plik w podanej przez użytkownika lokalizacji 
                strumienPliku = new FileStream(fullFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

                //Tworzymy obiekt FileInfo, który zawiera informacje o pliku
                infoPliku = new FileInfo(fullFilePath);

                //Ustawiamy flagę IsDestroyed na false, ponieważ plik został właśnie utworzony - NIE JEST ZNISZCZONY
                IsDestroyed = false;

                //Ustawiamy flagę _zamkniety na false, ponieważ obiekt nie jest zamknięty
                _zamkniety = false;
            }
            catch (IOException ioError)
            {
                //Wyjątek jeśli nie uda się utworzyć pliku w zadanej lokalizacji
                throw new IOException("Nie udało się utworzyć pliku tymczasowego pod wskazaną ścieżką.", ioError);
            }
        }

        //Metoda AddText umożliwia zapisanie tekstu do pliku jako bajty, jest to konkretniej kodowanie UTF-8
        public virtual void AddText(string tekst)
        {
            EnsureNotDisposed();

            //zmienna bytes przechowuje bajty tekstu zakodowanego w UTF-8 
            var bytes = Encoding.UTF8.GetBytes(tekst);

            //Zapisujemy bajty do pliku
            strumienPliku.Write(bytes, 0, bytes.Length);

            //Wymuszamy zapisanie na dysku, jest to ważne, aby nie stracić danych, opróżniamy bufor
            strumienPliku.Flush();
        }

        //Metoda Delete usuwa plik z dysku i ustawia flagę IsDestroyed
        public virtual void Delete()
        {
            //Sprawdzamy czy plik nie został już zniszczony
            if (!IsDestroyed)
            {
                //Zamykamy strumień pliku jeżeli jest otwarty
                strumienPliku?.Close();
                if (infoPliku.Exists)
                {
                    //Usuwamy plik z dysku jeżeli jeszcze istnieje 
                    infoPliku.Delete();
                }
                //Ustawiamy flagę IsDestroyed na true, ponieważ plik został usunięty
                IsDestroyed = true;
            }
        }

        //Metoda pomocnicza sprawdzająca czy obiekt nie został już zniszczony - rzuca wyjątek jeśli tak
        protected void EnsureNotDisposed()
        {
            //Jeżeli plik został zniszczony, zgłaszamy wyjątek ObjectDisposedException
            if (_zamkniety)
                throw new ObjectDisposedException(nameof(TempFile));
        }

        //Wzorzec Dispose - pozwala poprawnie zwolnić zasoby
        protected virtual void Dispose(bool disposing)
        {
            if (!_zamkniety)
            {
                if (disposing)
                {
                    //Zwalniamy strumień pliku
                    strumienPliku?.Dispose();
                }
                //Usuwamy plik z dysku
                Delete();
                _zamkniety = true;
            }
        }

        //Definiujemy "Finalizer", który wywołuje Dispose w przypadku braku jawnego zwolnienia zasobów
        ~TempFile()
        {
            //Wywołujemy Dispose, z parametrem false
            Dispose(false);
        }

        //Publiczna metoda Dispose - jawne zwolnienie zasobów
        public void Dispose()
        {
            //Wywołujemy Dispose, z parametrem true, jest to główna logika naszego zwalniania zasobów 
            Dispose(true);

            //Informujemy system, że nie ma potrzeby wywoływania finalizera
            GC.SuppressFinalize(this);
        }
    }
}