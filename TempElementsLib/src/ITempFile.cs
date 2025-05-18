using System.IO;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempFile - reprezentuje plik tymczasowy
    public interface ITempFile : ITempElement
    {
        //Właściwość FileStream - dostęp do strumienia bajtów pliku
        FileStream FileStream { get; }

        //Właściwość FileInfo - informacje o pliku
        FileInfo FileInfo { get; }

        //Metoda AddText - zapisuje tekst do pliku (jako bajty)
        void AddText(string value);
    }
}