using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ExcelDna.Integration;
using ExcelToSqlite.Utils;
using System.Text;

namespace ExcelToSqlite.Models;

public class Tasks
{
    [Key]
    public int Id { get; set; }
    public string TaskName { get; set; } = "-";
}
public class Details
{
    [Key]
    public int Id { get; set; }
    public int TasksId { get; set; }
    public string DetailName { get; set; } = "-";
}
public class Versions
{
    [Key]
    public string Version { get; set; } = "0";
    public string 内容 { get; set; } = "-";
    public string 更新日 { get; set; } = "2024/01/01";
}

public class Context : DbContext
{
    public DbSet<Tasks>? tasks { get; set; }
    public DbSet<Details>? details { get; set; }
    public DbSet<Versions>? versions { get; set; }
    private static DirectoryInfo DirectoryInfo = new(Path.GetDirectoryName(ExcelDnaUtil.XllPath)
                                                ?? new(AppDomain.CurrentDomain.BaseDirectory));
    private static string RootDBPath = "DataSource=" + Path.Combine(DirectoryInfo.FullName, @"Todo.db");
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
                    .LogTo(
                    message => Debug.WriteLine(message),
                    new[] { DbLoggerCategory.Database.Name },
                    LogLevel.Debug,
                    DbContextLoggerOptions.LocalTime)
                    .UseSqlite(RootDBPath);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { }
}

public class Todo
{
    public static void InitializeDB()
    {
        // データベースをマイグレーション
        using var context = new Context();
        context.Database.Migrate();
    }
    // 新たなレコードを追加
    public static void InsertTasks(object[,] taskArray)
    {
        List<Tasks> taskList = taskArray.ToGenericList(values => new Tasks
        {
            Id = Convert.ToInt32(values[0]),
            TaskName = (string)values[1]
        });
        using var context = new Context();
        context.tasks?.AddRange(taskList);
        context.SaveChanges();
    }
    // レコードを取得する
    public static object[,] SelectTasks(string name)
    {
        using var context = new Context();
        var tmp = context.tasks?.ToList();
        var list = tmp?
                    .Where(x => NormalizeString(x.TaskName).Contains(NormalizeString(name), StringComparison.OrdinalIgnoreCase))
                    .ToList();
        return list?.To2DArray() ?? throw new ArgumentNullException();
    }
    public static string NormalizeString(string input)
    {
        return input.Normalize(NormalizationForm.FormKC);
    }
}