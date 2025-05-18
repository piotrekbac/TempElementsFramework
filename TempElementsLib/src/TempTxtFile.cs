using System;
using System.IO;
using System.Text;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Klasa TempTxtFile służy do obsługi tymczasowych plików tekstowych, umożliwia wygodne operacje na tekście
    public class TempTxtFile : TempFile, ITempTxtFile
    {
        //Prywatne pole StreamWriter służące do zapisu tekstu do pliku
        private StreamWriter _zapis;

        //Prywatne pole StreamReader służące do odczytu tekstu z pliku
        private StreamReader _odczyt;

        //Konstruktor domyślny - tworzy plik tekstowy w katalogu tymczasowym
        public TempTxtFile() : base()
        {
            //Tworzymy strumienie do zapisu i odczytu tekstu z kodowaniem UTF-8
            _zapis = new StreamWriter(FileStream, Encoding.UTF8, 1024, leaveOpen: true);
            _odczyt = new StreamReader(FileStream, Encoding.UTF8, true, 1024, leaveOpen: true);
        }

        //Konstruktor z możliwością podania ścieżki do pliku tekstowego
        public TempTxtFile(string sciezka) : base(sciezka)
        {
            //Tworzymy strumienie do zapisu i odczytu tekstu z kodowaniem UTF-8
            _zapis = new StreamWriter(FileStream, Encoding.UTF8, 1024, leaveOpen: true);
            _odczyt = new StreamReader(FileStream, Encoding.UTF8, true, 1024, leaveOpen: true);
        }

        //Metoda Write pozwala zapisać tekst bez znaku nowej linii
        public void Write(string tekst)
        {
            //Sprawdzamy czy plik nie został już zniszczony
            EnsureNotDisposed();
            //Zapisujemy tekst do strumienia
            _zapis.Write(tekst);
            //Opróżniamy bufor zapisu, aby dane znalazły się na dysku
            _zapis.Flush();
            FileStream.Flush();
        }

        //Metoda WriteLine pozwala zapisać tekst z końcem linii
        public void WriteLine(string tekst)
        {
            //Sprawdzamy czy plik nie został już zniszczony
            EnsureNotDisposed();
            //Zapisujemy tekst z nową linią
            _zapis.WriteLine(tekst);
            //Opróżniamy bufor zapisu, aby dane znalazły się na dysku
            _zapis.Flush();
            FileStream.Flush();
        }

        //Metoda ReadLine odczytuje jedną linię tekstu od początku pliku
        public string ReadLine()
        {
            //Sprawdzamy czy plik nie został już zniszczony
            EnsureNotDisposed();
            //Ustawiamy pozycję odczytu na początek pliku
            if (FileStream.CanSeek)
                FileStream.Seek(0, SeekOrigin.Begin);
            //Odczytujemy jedną linię tekstu
            return _odczyt.ReadLine();
        }

        //Metoda ReadAllText odczytuje całą zawartość pliku jako tekst
        public string ReadAllText()
        {
            //Sprawdzamy czy plik nie został już zniszczony
            EnsureNotDisposed();
            //Ustawiamy pozycję odczytu na początek pliku
            if (FileStream.CanSeek)
                FileStream.Seek(0, SeekOrigin.Begin);
            //Odczytujemy cały tekst
            return _odczyt.ReadToEnd();
        }

        //Metoda Flush opróżnia bufory zapisu
        public void Flush()
        {
            //Sprawdzamy czy plik nie został już zniszczony
            EnsureNotDisposed();
            //Opróżniamy bufor zapisu i wymuszamy zapis na dysku
            _zapis.Flush();
            FileStream.Flush();
        }

        //Nadpisanie Dispose - poprawne zwalnianie zasobów tekstowych
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Zamykamy strumienie tekstowe
                _zapis?.Dispose();
                _odczyt?.Dispose();
            }
            //Wywołujemy zwalnianie zasobów z klasy bazowej
            base.Dispose(disposing);
        }
    }
}