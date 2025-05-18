using System.Collections.Generic;

//Piotr Bacior - 15 722 - Informatyka stosowana - WSEI

namespace TempElementsLib
{
    //Interfejs ITempElementsStack rozszerza ITempElements o wycofywanie ostatniego elementu
    public interface ITempElementsStack : ITempElements
    {
        //Pop - zwalnia i usuwa ostatnio dodany element (undo)
        ITempElement Pop();
    }
}