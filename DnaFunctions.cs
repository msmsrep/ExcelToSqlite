using ExcelDna.Integration;
using ExcelToSqlite.Models;

namespace ExcelToSqlite;
public static class DnaFunctions
{
    [ExcelFunction]
    public static string SayHello(string name)
    {
        return "Hello " + name;
    }
    // データベースをマイグレーション
    [ExcelFunction]
    public static string DnaInitialize()
    {
        try
        {
            Todo.InitializeDB();
            return "true";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    // レコードを追加
    [ExcelFunction]
    public static string DnaInsert(object[,] values)
    {
        try
        {
            Todo.InsertTasks(values);
            return "true";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }
    // レコードを取得
    [ExcelFunction]
    public static object[,] DnaSelect(string name)
    {
        try
        {
            return Todo.SelectTasks(name);
        }
        catch (Exception ex)
        {
            return new object[,] { { "エラー" }, { ex.ToString() } };
        }
    }
}