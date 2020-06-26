// Translation lookaside buffer 
// Traduce de direcciones de los programas de usuario
// a direcciones del procesador. Esto debido a que hay que pasar de 
// direcciones de memoria indices de los arreglos de la memoria. 
public class TBL 
{

    public static int DataDirToIndex(int direction) => direction / 4;
    public static int InstDirToIndex(int direction) => (direction / 4) - MemoryConstants.DataMemorySize;
    public static int InstIndexToDir(int index) => 4 * (index + MemoryConstants.DataMemorySize);

}
