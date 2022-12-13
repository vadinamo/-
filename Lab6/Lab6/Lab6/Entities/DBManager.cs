using System.Data;
using Npgsql;
namespace Lab6.Entities;

public class DBManager
{
    public static NpgsqlCommand getCommand()
    {
        var connection =
            new NpgsqlConnection("Server=localhost; Port=5432; Database=real_estate_agency; Username=postgres; Password=123;");
        connection.Open();
        var command = new NpgsqlCommand();
        command.Connection = connection;
        command.CommandType = CommandType.Text;
        return command;
    }
}