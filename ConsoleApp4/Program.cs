using System;
using System.Data;
using Npgsql;

class Program
{
    static DataTable studentsTable;
    static string connectionString = "Host=localhost;Port=5432;Database=university;Username=postgres;Password=postgres";

    static void Main()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            var adapter = new NpgsqlDataAdapter("SELECT id, name, age FROM students", connection);
            studentsTable = new DataTable("students");
            adapter.Fill(studentsTable);

            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== Меню фильтрации и сортировки ===");
                Console.WriteLine("1. Показать все данные");
                Console.WriteLine("2. Показать студентов старше 18 лет");
                Console.WriteLine("3. Отсортировать по имени");
                Console.WriteLine("4. Изменить критерий фильтрации");
                Console.WriteLine("5. Проверить данные в PostgreSQL");
                Console.WriteLine("6. Сбросить фильтр и сортировку");
                Console.WriteLine("0. Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAll();
                        break;
                    case "2":
                        FilterByAge(18);
                        break;
                    case "3":
                        SortByName();
                        break;
                    case "4":
                        ChangeFilter();
                        break;
                    case "5":
                        CheckDatabase(connection);
                        break;
                    case "6":
                        ResetFilter();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }
    }

    static void ShowAll()
    {
        studentsTable.DefaultView.RowFilter = null;
        studentsTable.DefaultView.Sort = null;
        DisplayView();
    }

    static void FilterByAge(int minAge)
    {
        studentsTable.DefaultView.RowFilter = $"age > {minAge}";
        Console.WriteLine($"\nФильтр: возраст > {minAge}");
        DisplayView();
    }

    static void SortByName()
    {
        studentsTable.DefaultView.Sort = "name ASC";
        Console.WriteLine("\nСортировка: по имени (A-Z)");
        DisplayView();
    }

    static void ChangeFilter()
    {
        Console.WriteLine("\nВыберите критерий:");
        Console.WriteLine("1. Возраст > 20");
        Console.WriteLine("2. Возраст < 22");
        Console.WriteLine("3. Имя содержит 'а' (регистронезависимо)");
        Console.WriteLine("4. Возраст между 19 и 23");
        Console.Write("Выбор: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                studentsTable.DefaultView.RowFilter = "age > 20";
                break;
            case "2":
                studentsTable.DefaultView.RowFilter = "age < 22";
                break;
            case "3":
                studentsTable.DefaultView.RowFilter = "name LIKE '%а%'";
                break;
            case "4":
                studentsTable.DefaultView.RowFilter = "age >= 19 AND age <= 23";
                break;
            default:
                Console.WriteLine("Неверный выбор.");
                return;
        }

        DisplayView();
    }

    static void CheckDatabase(NpgsqlConnection connection)
    {
        Console.WriteLine("\n=== Данные в PostgreSQL (без фильтра) ===");
        using (var cmd = new NpgsqlCommand("SELECT id, name, age FROM students ORDER BY id", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["id"]}, Имя: {reader["name"]}, Возраст: {reader["age"]}");
            }
        }
        Console.WriteLine("\nВывод: RowFilter и Sort изменяют только представление DataView,");
        Console.WriteLine("данные в DataTable и PostgreSQL остаются без изменений.");
    }

    static void ResetFilter()
    {
        studentsTable.DefaultView.RowFilter = null;
        studentsTable.DefaultView.Sort = null;
        Console.WriteLine("\nФильтр и сортировка сброшены.");
        DisplayView();
    }

    static void DisplayView()
    {
        Console.WriteLine($"\nВсего строк в DataTable: {studentsTable.Rows.Count}");
        Console.WriteLine($"Строк в представлении: {studentsTable.DefaultView.Count}");
        Console.WriteLine($"RowFilter: {studentsTable.DefaultView.RowFilter ?? "нет"}");
        Console.WriteLine($"Sort: {studentsTable.DefaultView.Sort ?? "нет"}");
        Console.WriteLine();

        foreach (DataRowView row in studentsTable.DefaultView)
        {
            Console.WriteLine($"ID: {row["id"]}, Имя: {row["name"]}, Возраст: {row["age"]}");
        }
    }
}