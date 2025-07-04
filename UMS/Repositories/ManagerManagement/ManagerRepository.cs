using System.Data;
using Dapper;
using Microsoft.AspNetCore.Server.HttpSys;
using UMS.DynamicParametersExtension;
using UMS.Models.Entities;
using UMS.Models.Manager;
using UMS.Services;

namespace UMS.Repositories.ManagerManagement;

public class ManagerRepository(IDapperRepository repository) : IManagerRepository
{
    public List<GetManagerQueryResponse> GetAllManagers(DataTableRequest request)
    {
        DynamicParameters parameters = new();
        parameters.AddDefaultPaginationParameters(request);
        var result = repository.Query<GetManagerQueryResponse>(StoredProcedures.GET_ALL_MANAGERS, parameters);
        return result.ToList();
    }

    public GetManagerByIdQueryResponse GetManagerById(int id)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", id);
        var result = repository.QuerySingleOrDefault<GetManagerByIdQueryResponse>(StoredProcedures.GET_MANAGER_BY_ID, parameters);
        return result;
    }

    public int UpdateManager(int id, AddManager manager)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", id);
        parameters.Add("@FullName", manager.FullName);
        parameters.Add("@Email", manager.Email);
        parameters.Add("@UserName",manager.Username);
        parameters.Add("@Password", manager.Password);
        parameters.Add("@DesignationId",manager.DesignationId);
        parameters.Add("@Result",dbType:DbType.Int32,direction:ParameterDirection.Output);
        repository.Execute(StoredProcedures.UPDATE_MANAGER, parameters);
        var result  = parameters.Get<int>("@Result");
        return result;
    }

    public int DeleteManager(int id)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", id);
        parameters.Add("@Result", dbType:DbType.Int32,direction:ParameterDirection.Output);
        repository.Execute(StoredProcedures.DELETE_MANAGER, parameters);
        
        var result = parameters.Get<int>("@Result");
        return result;
    }

    public int AddManager(AddManager manager)
    {
        DynamicParameters parameters = new();
        parameters.Add("@FullName", manager.FullName);
        parameters.Add("@Email", manager.Email);
        parameters.Add("@UserName", manager.Username);
        parameters.Add("@DesignationId", manager.DesignationId);
        parameters.Add("@Password", manager.Password);
        parameters.Add("@RoleId",UMS.Enums.Roles.Employee);
        parameters.Add("@Result", dbType:DbType.Int32,direction:ParameterDirection.Output);
        
        repository.Execute(StoredProcedures.ADD_MANAGER, parameters);
        
        var result = parameters.Get<int>("@Result");
        return result;
    }
}