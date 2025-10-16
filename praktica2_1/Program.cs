using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WarehouseApp
{
    class Program
    {
        static SqlConnection conn = null;
        static SqlConnection connection;
        static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=Sklad;Trusted_Connection=True;";

        static void DisplayProductsByCategory()
        {
            if (!CheckConnection()) return;

            Console.Write("Введите название категории (типа товара): ");
            string category = Console.ReadLine();

            string query = @"
            SELECT p.ProductID, p.ProductName, pt.TypeName, p.Quantity, p.Cost, s.SupplierName
            FROM Products p
            INNER JOIN ProductTypes pt ON p.TypeID = pt.TypeID
            LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID
            WHERE pt.TypeName = @Category";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Category", category);

            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine($"\nТовары категории '{category}':");
            Console.WriteLine("ID \tНазвание \tТип \tКол-во \tСебестоимость \tПоставщик");

            bool hasRows = false;
            while (reader.Read())
            {
                hasRows = true;
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string type = reader.GetString(2);
                int quantity = reader.GetInt32(3);
                decimal cost = reader.GetDecimal(4);
                string supplier = reader.IsDBNull(5) ? "Нет" : reader.GetString(5);

                Console.WriteLine($"{id}\t{name}\t{type}\t{quantity}\t{cost}\t{supplier}");
            }

            if (!hasRows)
            {
                Console.WriteLine("Товары данной категории не найдены.");
            }

            reader.Close();
        }

        static void DisplayProductsBySupplier()
        {
            if (!CheckConnection()) return;

            Console.Write("Введите название поставщика: ");
            string supplierName = Console.ReadLine();

            string query = @"
            SELECT p.ProductID, p.ProductName, pt.TypeName, p.Quantity, p.Cost, s.SupplierName
            FROM Products p
            INNER JOIN ProductTypes pt ON p.TypeID = pt.TypeID
            INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
            WHERE s.SupplierName = @SupplierName";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SupplierName", supplierName);

            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine($"\nТовары поставщика '{supplierName}':");
            Console.WriteLine("ID \tНазвание \tТип \tКол-во \tСебестоимость \tПоставщик");

            bool hasRows = false;
            while (reader.Read())
            {
                hasRows = true;
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string type = reader.GetString(2);
                int quantity = reader.GetInt32(3);
                decimal cost = reader.GetDecimal(4);
                string supplier = reader.GetString(5);

                Console.WriteLine($"{id}\t{name}\t{type}\t{quantity}\t{cost}\t{supplier}");
            }

            if (!hasRows)
            {
                Console.WriteLine("Товары данного поставщика не найдены.");
            }
            reader.Close();
        }

        static void DisplayOldestProduct()
        {
            if (!CheckConnection()) return;

            string query = @"
            SELECT TOP 1 ProductName, DateAdded
            FROM Products
            WHERE DateAdded IS NOT NULL
            ORDER BY DateAdded ASC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nСамый старый товар на складе:");

            if (reader.Read())
            {
                string name = reader.GetString(0);
                DateTime dateAdded = reader.GetDateTime(1);
                Console.WriteLine($"Название: {name}, Дата поступления: {dateAdded:yyyy-MM-dd}");
            }
            else
            {
                Console.WriteLine("Данные о товарах отсутствуют.");
            }
            reader.Close();
        }

        static void DisplayAverageQuantityPerProductType()
        {
            if (!CheckConnection()) return;

            string query = @"
            SELECT pt.TypeName, AVG(CAST(p.Quantity AS FLOAT)) AS AverageQuantity
            FROM Products p
            INNER JOIN ProductTypes pt ON p.TypeID = pt.TypeID
            GROUP BY pt.TypeName";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nСреднее количество товаров по типу:");

            Console.WriteLine("Тип товара \tСреднее количество");

            while (reader.Read())
            {
                string typeName = reader.GetString(0);
                double avgQuantity = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);

                Console.WriteLine($"{typeName}\t{avgQuantity:F2}");
            }
            reader.Close();
        }
        static void ConnectToDatabase()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Уже подключены к базе данных.");
                return;
            }

            conn = new SqlConnection(connectionString);
            try
            {
                conn.Open();
                Console.WriteLine("Успешное подключение к базе данных «Склад».");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Ошибка при подключении: " + ex.Message);
                conn = null;
            }
        }

        static void DisconnectFromDatabase()
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
                Console.WriteLine("Подключение закрыто.");
            }
            else
            {
                Console.WriteLine("Нет активного подключения.");
            }
        }

        static void DisplayAllProducts()
        {
            if (!CheckConnection()) return;

            string query = @"
                SELECT p.ProductID, p.ProductName, pt.TypeName, p.Quantity, p.Cost, s.SupplierName
                FROM Products p
                INNER JOIN ProductTypes pt ON p.TypeID = pt.TypeID
                LEFT JOIN Suppliers s ON p.SupplierID = s.SupplierID";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nВсе товары:");
            Console.WriteLine("ID \tНазвание \tТип \tКол-во \tСебестоимость \tПоставщик");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string type = reader.GetString(2);
                int quantity = reader.GetInt32(3);
                decimal cost = reader.GetDecimal(4);
                string supplier = reader.IsDBNull(5) ? "Нет" : reader.GetString(5);

                Console.WriteLine($"{id}\t{name}\t{type}\t{quantity}\t{cost}\t{supplier}");
            }
            reader.Close();
        }

        static void DisplayAllProductTypes()
        {
            if (!CheckConnection()) return;

            string query = "SELECT TypeID, TypeName FROM ProductTypes";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nВсе типы товаров:");
            Console.WriteLine("ID \tНазвание");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                Console.WriteLine($"{id}\t{name}");
            }
            reader.Close();
        }

        static void DisplayAllSuppliers()
        {
            if (!CheckConnection()) return;

            string query = "SELECT SupplierID, SupplierName FROM Suppliers";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nВсе поставщики:");
            Console.WriteLine("ID \tНазвание");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                Console.WriteLine($"{id}\t{name}");
            }
            reader.Close();
        }

        static void DisplayProductWithMaxQuantity()
        {
            if (!CheckConnection()) return;

            string query = @"
                SELECT TOP 1 ProductName, Quantity
                FROM Products
                ORDER BY Quantity DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nТовар с максимальным количеством:");

            if (reader.Read())
            {
                string name = reader.GetString(0);
                int quantity = reader.GetInt32(1);
                Console.WriteLine($"Название: {name}, Количество: {quantity}");
            }
            else
            {
                Console.WriteLine("Товары не найдены.");
            }
            reader.Close();
        }

        static void DisplayProductWithMinQuantity()
        {
            if (!CheckConnection()) return;

            string query = @"
                SELECT TOP 1 ProductName, Quantity
                FROM Products
                ORDER BY Quantity ASC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nТовар с минимальным количеством:");

            if (reader.Read())
            {
                string name = reader.GetString(0);
                int quantity = reader.GetInt32(1);
                Console.WriteLine($"Название: {name}, Количество: {quantity}");
            }
            else
            {
                Console.WriteLine("Товары не найдены.");
            }
            reader.Close();
        }

        static void DisplayProductWithMinCost()
        {
            if (!CheckConnection()) return;

            string query = @"
                SELECT TOP 1 ProductName, Cost
                FROM Products
                ORDER BY Cost ASC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nТовар с минимальной себестоимостью:");

            if (reader.Read())
            {
                string name = reader.GetString(0);
                decimal cost = reader.GetDecimal(1);
                Console.WriteLine($"Название: {name}, Себестоимость: {cost}");
            }
            else
            {
                Console.WriteLine("Товары не найдены.");
            }
            reader.Close();
        }

        static void DisplayProductWithMaxCost()
        {
            if (!CheckConnection()) return;

            string query = @"
                SELECT TOP 1 ProductName, Cost
                FROM Products
                ORDER BY Cost DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nТовар с максимальной себестоимостью:");

            if (reader.Read())
            {
                string name = reader.GetString(0);
                decimal cost = reader.GetDecimal(1);
                Console.WriteLine($"Название: {name}, Себестоимость: {cost}");
            }
            else
            {
                Console.WriteLine("Товары не найдены.");
            }
            reader.Close();
        }

        static bool CheckConnection()
        {
            if (conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Сначала подключитесь к базе данных.");
                return false;
            }
            return true;
        }

        static void InsertNewProduct()
        {
            Console.WriteLine("Введите название товара:");
            string productName = Console.ReadLine();

            Console.WriteLine("Введите цену:");
            decimal price;
            while (!decimal.TryParse(Console.ReadLine(), out price))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите количество:");
            int quantity;
            while (!int.TryParse(Console.ReadLine(), out quantity))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите ID типа товара:");
            int typeId;
            while (!int.TryParse(Console.ReadLine(), out typeId))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите ID поставщика:");
            int supplierId;
            while (!int.TryParse(Console.ReadLine(), out supplierId))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            string query = "INSERT INTO Products (Name, Price, Quantity, TypeId, SupplierId) VALUES (@Name, @Price, @Quantity, @TypeId, @SupplierId)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", productName);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@TypeId", typeId);
                cmd.Parameters.AddWithValue("@SupplierId", supplierId);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Товар успешно добавлен.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при добавлении товара: " + ex.Message);
                }
            }
        }
        static void InsertNewProductType()
        {
            Console.WriteLine("Введите название типа товара:");
            string typeName = Console.ReadLine();

            string query = "INSERT INTO ProductTypes (TypeName) VALUES (@TypeName)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@TypeName", typeName);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Тип товара успешно добавлен.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при добавлении типа товара: " + ex.Message);
                }
            }
        }

        static void InsertNewSupplier()
        {
            Console.WriteLine("Введите название поставщика:");
            string supplierName = Console.ReadLine();

            string query = "INSERT INTO Suppliers (SupplierName) VALUES (@SupplierName)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@SupplierName", supplierName);
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Поставщик успешно добавлен.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при добавлении поставщика: " + ex.Message);
                }
            }
        }
        static void UpdateProduct()
        {
            Console.WriteLine("Введите ID товара для обновления:");
            int productId;
            while (!int.TryParse(Console.ReadLine(), out productId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            Console.WriteLine("Введите новое название товара:");
            string newName = Console.ReadLine();

            Console.WriteLine("Введите новую цену:");
            decimal newPrice;
            while (!decimal.TryParse(Console.ReadLine(), out newPrice))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите новое количество:");
            int newQuantity;
            while (!int.TryParse(Console.ReadLine(), out newQuantity))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите новых ID типа товара:");
            int newTypeId;
            while (!int.TryParse(Console.ReadLine(), out newTypeId))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            Console.WriteLine("Введите нового ID поставщика:");
            int newSupplierId;
            while (!int.TryParse(Console.ReadLine(), out newSupplierId))
            {
                Console.WriteLine("Некорректное значение. Попробуйте снова:");
            }

            string query = @"
            UPDATE Products
            SET Name = @Name, Price = @Price, Quantity = @Quantity, TypeId = @TypeId, SupplierId = @SupplierId
            WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", newName);
                cmd.Parameters.AddWithValue("@Price", newPrice);
                cmd.Parameters.AddWithValue("@Quantity", newQuantity);
                cmd.Parameters.AddWithValue("@TypeId", newTypeId);
                cmd.Parameters.AddWithValue("@SupplierId", newSupplierId);
                cmd.Parameters.AddWithValue("@Id", productId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Товар успешно обновлен.");
                    else
                        Console.WriteLine("Товар с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка обновления: " + ex.Message);
                }
            }
        }
        static void UpdateSupplier()
        {
            Console.WriteLine("Введите ID поставщика для обновления:");
            int supplierId;
            while (!int.TryParse(Console.ReadLine(), out supplierId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            Console.WriteLine("Введите новое название поставщика:");
            string newName = Console.ReadLine();

            string query = "UPDATE Suppliers SET SupplierName = @Name WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Name", newName);
                cmd.Parameters.AddWithValue("@Id", supplierId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Поставщик успешно обновлен.");
                    else
                        Console.WriteLine("Поставщик с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка обновления: " + ex.Message);
                }
            }
        }
        static void UpdateProductType()
        {
            Console.WriteLine("Введите ID типа товара для обновления:");
            int typeId;
            while (!int.TryParse(Console.ReadLine(), out typeId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            Console.WriteLine("Введите новое название типа:");
            string newTypeName = Console.ReadLine();

            string query = "UPDATE ProductTypes SET TypeName = @TypeName WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@TypeName", newTypeName);
                cmd.Parameters.AddWithValue("@Id", typeId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Тип товара успешно обновлен.");
                    else
                        Console.WriteLine("Тип товара с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка обновления: " + ex.Message);
                }
            }
        }
        static void DeleteProduct()
        {
            Console.WriteLine("Введите ID товара для удаления:");
            int productId;
            while (!int.TryParse(Console.ReadLine(), out productId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            string query = "DELETE FROM Products WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Id", productId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Товар успешно удален.");
                    else
                        Console.WriteLine("Товар с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка удаления: " + ex.Message);
                }
            }
        }
        static void DeleteSupplier()
        {
            Console.WriteLine("Введите ID поставщика для удаления:");
            int supplierId;
            while (!int.TryParse(Console.ReadLine(), out supplierId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            string query = "DELETE FROM Suppliers WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Id", supplierId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Поставщик успешно удален.");
                    else
                        Console.WriteLine("Поставщик с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка удаления: " + ex.Message);
                }
            }
        }
        static void DeleteProductType()
        {
            Console.WriteLine("Введите ID типа товара для удаления:");
            int typeId;
            while (!int.TryParse(Console.ReadLine(), out typeId))
            {
                Console.WriteLine("Некорректный ID. Попробуйте снова:");
            }

            string query = "DELETE FROM ProductTypes WHERE Id = @Id";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Id", typeId);

                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                        Console.WriteLine("Тип товара успешно удален.");
                    else
                        Console.WriteLine("Тип товара с таким ID не найден.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка удаления: " + ex.Message);
                }
            }
        }
        static void ShowSupplierWithMaxProducts()
        {
            string query = @"
                SELECT TOP 1 s.Id, s.SupplierName, SUM(p.Quantity) AS TotalQuantity
                FROM Suppliers s
                JOIN Products p ON s.Id = p.SupplierId
                GROUP BY s.Id, s.SupplierName
                ORDER BY TotalQuantity DESC";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Поставщик с наибольшим количеством товаров:");
                        Console.WriteLine($"ID: {reader["Id"]}");
                        Console.WriteLine($"Название: {reader["SupplierName"]}");
                        Console.WriteLine($"Общее количество товаров: {reader["TotalQuantity"]}");
                    }
                    else
                    {
                        Console.WriteLine("Данные не найдены.");
                    }
                }
            }
        }
        static void ShowSupplierWithMinProducts()
        {
            string query = @"
                SELECT TOP 1 s.Id, s.SupplierName, SUM(p.Quantity) AS TotalQuantity
                FROM Suppliers s
                JOIN Products p ON s.Id = p.SupplierId
                GROUP BY s.Id, s.SupplierName
                ORDER BY TotalQuantity ASC";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Поставщик с наименьшим количеством товаров:");
                        Console.WriteLine($"ID: {reader["Id"]}");
                        Console.WriteLine($"Название: {reader["SupplierName"]}");
                        Console.WriteLine($"Общее количество товаров: {reader["TotalQuantity"]}");
                    }
                    else
                    {
                        Console.WriteLine("Данные не найдены.");
                    }
                }
            }
        }
        static void ShowProductTypeWithMaxProducts()
        {
            string query = @"
                SELECT TOP 1 pt.Id, pt.TypeName, SUM(p.Quantity) AS TotalQuantity
                FROM ProductTypes pt
                JOIN Products p ON pt.Id = p.TypeId
                GROUP BY pt.Id, pt.TypeName
                ORDER BY TotalQuantity DESC";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Тип товара с наибольшим количеством товаров:");
                        Console.WriteLine($"ID: {reader["Id"]}");
                        Console.WriteLine($"Название: {reader["TypeName"]}");
                        Console.WriteLine($"Общее количество товаров: {reader["TotalQuantity"]}");
                    }
                    else
                    {
                        Console.WriteLine("Данные не найдены.");
                    }
                }
            }
        }
        static void ShowProductTypeWithMinProducts()
        {
            string query = @"
                SELECT TOP 1 pt.Id, pt.TypeName, SUM(p.Quantity) AS TotalQuantity
                FROM ProductTypes pt
                JOIN Products p ON pt.Id = p.TypeId
                GROUP BY pt.Id, pt.TypeName
                ORDER BY TotalQuantity ASC";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Тип товара с наименьшим количеством товаров:");
                        Console.WriteLine($"ID: {reader["Id"]}");
                        Console.WriteLine($"Название: {reader["TypeName"]}");
                        Console.WriteLine($"Общее количество товаров: {reader["TotalQuantity"]}");
                    }
                    else
                    {
                        Console.WriteLine("Данные не найдены.");
                    }
                }
            }
        }
        static void ShowProductsDeliveredBefore(int daysAgo)
        {
            string query = @"
                SELECT Id, Name, DeliveryDate
                FROM Products
                WHERE DeliveryDate <= DATEADD(day, -@DaysAgo, GETDATE())";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@DaysAgo", daysAgo);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine($"Товары, поставки которых прошли более {daysAgo} дней назад:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["Id"]}, Название: {reader["Name"]}, Дата поставки: {reader["DeliveryDate"]}");
                    }
                }
            }
        }
        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\nДоступные команды:");
                Console.WriteLine("1 - Подключиться к базе данных");
                Console.WriteLine("2 - Отключиться от базы данных");
                Console.WriteLine("3 - Показать всю информацию о товаре");
                Console.WriteLine("4 - Показать все типы товаров");
                Console.WriteLine("5 - Показать всех поставщиков");
                Console.WriteLine("6 - Показать товар с максимальным количеством");
                Console.WriteLine("7 - Показать товар с минимальным количеством");
                Console.WriteLine("8 - Показать товар с минимальной себестоимостью");
                Console.WriteLine("9 - Показать товар с максимальной себестоимостью");
                Console.WriteLine("до 22 - Выйти");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ConnectToDatabase();
                        break;
                    case "2":
                        DisconnectFromDatabase();
                        break;
                    case "3":
                        DisplayAllProducts();
                        break;
                    case "4":
                        DisplayAllProductTypes();
                        break;
                    case "5":
                        DisplayAllSuppliers();
                        break;
                    case "6":
                        DisplayProductWithMaxQuantity();
                        break;
                    case "7":
                        DisplayProductWithMinQuantity();
                        break;
                    case "8":
                        DisplayProductWithMinCost();
                        break;
                    case "9":
                        DisplayProductWithMaxCost();
                        break;
                    case "10":
                        DisplayProductsByCategory();
                        break;
                    case "11":
                        DisplayProductsBySupplier();
                        break;
                    case "12":
                        DisplayOldestProduct();
                        break;
                    case "13":
                        DisplayAverageQuantityPerProductType();
                        break;
                    case "14":
                        InsertNewProduct();
                        break;
                    case "15":
                        InsertNewProductType();
                        break;
                    case "16":
                        InsertNewSupplier();
                        break;
                    case "17":
                        UpdateProduct();
                        break;
                    case "18":
                        UpdateSupplier();
                        break;
                    case "19":
                        UpdateProductType();
                        break;
                    case "20":
                        DeleteProduct();
                        break;
                    case "21":
                        DeleteSupplier();
                        break;
                    case "22":
                        DeleteProductType();
                        break;
                    case "23":
                        ShowSupplierWithMaxProducts();
                        break;
                    case "24":
                        ShowSupplierWithMinProducts();
                        break;
                    case "25":
                        ShowProductTypeWithMaxProducts();
                        break;
                    case "26":
                        ShowProductTypeWithMinProducts();
                        break;
                    case "27":
                        Console.WriteLine("Введите количество дней:");
                        if (int.TryParse(Console.ReadLine(), out int days))
                        {
                            ShowProductsDeliveredBefore(days);
                        }
                        else
                        {
                            Console.WriteLine("Некорректное число дней.");
                        }
                        break;
                    case "0":
                        Console.WriteLine("Завершение работы программы.");
                        return;
                    default:
                        Console.WriteLine("Некорректная команда. Попробуйте снова.");
                        break;
                }
            }
        }
    }
}