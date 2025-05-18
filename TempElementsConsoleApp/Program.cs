using System;
using System.IO;
using TempElementsLib;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Piotr Bacior - 15 722 - WSEI Kraków");
            //Testujemy TempFile z domyślnym konstruktorem, zapisujemy tekst i odczytujemy go
            using (var tempFile = new TempFile())
            {
                Console.WriteLine($"Plik tymczasowy utworzony: {tempFile.Path}");
                tempFile.AddText("Witaj, pliku tymczasowy!\n");
                tempFile.FileStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(tempFile.FileStream, leaveOpen: true))
                {
                    //Odczytujemy zawartość pliku
                    Console.WriteLine("Odczyt z pliku: " + reader.ReadToEnd());
                }
            }
            Console.WriteLine("Plik tymczasowy został zwolniony i powinien być usunięty.");

            //Testujemy TempFile z konstruktorem ze ścieżką, próbujemy zapisać tekst do pliku w wybranej lokalizacji
            try
            {
                var tempFile2 = new TempFile(Path.Combine(Path.GetTempPath(), "mojplik.tmp"));
                tempFile2.AddText("Test ścieżki własnej pliku.");
                tempFile2.Dispose();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Błąd przy własnej ścieżce pliku: " + ex.Message);
            }

            //Próba utworzenia pliku w nieistniejącej lokalizacji, powinno rzucić wyjątek
            try
            {
                var tempFile3 = new TempFile(@"Z:\nieistniejacy\plik.tmp");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oczekiwany błąd dla niepoprawnej ścieżki: " + ex.Message);
            }

            //Dostęp po Dispose - powinien zgłosić wyjątek ObjectDisposedException
            var tempFile4 = new TempFile();
            tempFile4.Dispose();
            try
            {
                tempFile4.AddText("To powinno rzucić wyjątek ObjectDisposedException");
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Obsłużono wyjątek ObjectDisposedException zgodnie z oczekiwaniem.");
            }

            //Test try-catch bez using, ręczne zarządzanie cyklem życia
            TempFile tempFile5 = null;
            try
            {
                tempFile5 = new TempFile();
                tempFile5.AddText("Test bloku try-catch\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wyjątek w bloku try-catch: " + ex.Message);
            }
            finally
            {
                tempFile5?.Dispose();
            }

            //Test TempTxtFile z domyślnym konstruktorem, zapis i odczyt wielu linii tekstu
            using (var txtFile = new TempTxtFile())
            {
                txtFile.WriteLine("Linia 1");
                txtFile.WriteLine("Linia 2");
                txtFile.Flush();
                Console.WriteLine("Zawartość pliku TempTxtFile:");
                Console.WriteLine(txtFile.ReadAllText());
            }

            //Test TempDir - tworzymy katalog tymczasowy, zapisujemy w nim plik tekstowy i sprawdzamy czy katalog istnieje
            using (var tempDir = new TempDir())
            {
                Console.WriteLine($"Katalog tymczasowy utworzony: {tempDir.Path}");
                var filePath = System.IO.Path.Combine(tempDir.Path, "plik.txt");
                File.WriteAllText(filePath, "Zawartość pliku w katalogu tymczasowym.");
                Console.WriteLine("Czy katalog istnieje: " + Directory.Exists(tempDir.Path));
            }

            //Test TempElementsList - dodajemy plik i katalog do kolekcji, usuwamy plik, sprawdzamy ilość elementów po czyszczeniu
            using (var elements = new TempElementsList())
            {
                var f = elements.AddElement<TempFile>();
                Console.WriteLine($"Dodano element (plik): {f.Path}");
                var d = elements.AddElement<TempDir>();
                Console.WriteLine($"Dodano element (katalog): {d.Path}");
                f.Dispose();
                elements.RemoveDestroyed();
                Console.WriteLine("Liczba elementów po RemoveDestroyed: " + elements.Elements.Count);
            }

            Console.WriteLine("\n--- TEST STOSU (z pliku o nazwie TempElementsStack) ---");
            using (var stack = new TempElementsStack())
            {
                //Dodajemy pliki tymczasowe do stosu
                var plik1 = stack.AddElement<TempFile>();
                plik1.AddText("Pierwsza wersja danych\n");
                var plik2 = stack.AddElement<TempFile>();
                plik2.AddText("Druga wersja danych\n");

                Console.WriteLine("Elementy na stosie:");
                foreach (var elem in stack.Elements)
                    Console.WriteLine(elem.Path);

                //Wycofanie ostatniej operacji - usuwamy plik2
                var usuniety = stack.Pop();
                Console.WriteLine($"Usunięto (undo): {usuniety.Path}");

                //Został tylko pierwszy plik
                Console.WriteLine("Stan stosu po wycofaniu:");
                foreach (var elem in stack.Elements)
                    Console.WriteLine(elem.Path);

                //Dodaj katalog tymczasowy i sprawdź undo
                var kat = stack.AddElement<TempDir>();
                Console.WriteLine($"Dodano katalog: {kat.Path}");
                stack.Pop();
                Console.WriteLine("Katalog usunięty przez undo.");
            }
        }
    }
}