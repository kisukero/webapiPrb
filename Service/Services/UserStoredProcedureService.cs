using System.Data;
using System.Data.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class UserStoredProcedureService(InventoryDbContext db) : IUserService
{
    public async Task<ApplicationUser?> GetUser(string userName)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "dbo.csp_Inventory_User_Get";
        command.CommandType = CommandType.StoredProcedure;
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@UserName";
        parameter.Value = userName;
        command.Parameters.Add(parameter);

        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync()
            ? new ApplicationUser { Id = reader.GetInt32(0), UserName = reader.GetString(1), FullName = reader.GetString(2), PasswordHash = reader.GetString(3), IsActive = reader.GetBoolean(4) }
            : null;
    }
}
