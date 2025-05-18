using System.IO;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempTxtFile - plik tymczasowy przeznaczony do obsługi tekstowej
    public interface ITempTxtFile : ITempFile
    {
        //Metoda Write - zapisuje tekst bez końca linii
        void Write(string text);

        //Metoda WriteLine - zapisuje tekst z końcem linii
        void WriteLine(string text);

        //Metoda ReadLine - odczytuje jedną linię tekstu z pliku
        string ReadLine();

        //Metoda ReadAllText - odczytuje całą zawartość pliku jako tekst
        string ReadAllText();

        //Metoda Flush - opróżnia bufory zapisu
        void Flush();
    }
}