using System.Reflection;

namespace ExcelToSqlite.Utils;

public static class ExtensionsLinq
{
    // リストを2次元配列に変換する
    public static object[,] To2DArray<T>(this List<T> list)
    {
        if (list.Count == 0) return new object[,] { { 0 }, { "" } };

        PropertyInfo[] properties = typeof(T).GetProperties();
        int rows = list.Count;
        int cols = properties.Length;
        var result = new object[rows, cols];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[r, c] = properties[c].GetValue(list[r]) ?? "";
            }
        }
        return result;
    }
    // 2次元配列をリストに変換する
    public static List<T> ToGenericList<T>(this object[,] array, Func<object[], T> createInstance)
    {
        var list = new List<T>();
        for (int i = 0; i < array.GetLength(0); i++)
        {
            object[] values = new object[array.GetLength(1)];
            for (int j = 0; j < array.GetLength(1); j++)
            {
                values[j] = array[i, j];
            }
            list.Add(createInstance(values));
        }
        return list;
    }
}