using System.IO;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempDir - reprezentuje katalog tymczasowy
    public interface ITempDir : ITempElement
    {
        //Właściwość DirectoryInfo - informacje o katalogu tymczasowym
        DirectoryInfo DirectoryInfo { get; }
    }
}