using System.Data;
using System.Net;
using Dapper;
using UMS.DynamicParametersExtension;
using UMS.Models.Employee;
using UMS.Models.Entities;

namespace UMS.Repositories.EmployeeManagement;

public class EmployeeRepository(IDapperRepository repository) : IEmployeeRepository
{
    public async Task<List<Employee>> GetAllEmployees(DataTableRequest request)
    {
        DynamicParameters parameters = new();
        parameters.AddDefaultPaginationParameters(request);
        var result = await repository.QueryAsync<Employee>(StoredProcedures.GET_ALL_EMPLOYEE, parameters);
        return result.ToList();
    }

    public async Task<AddEmployee> GetEmployeeById(int id)
    {

        DynamicParameters parameters = new();
        parameters.Add("@id", id);
        var result = await repository.QuerySingleOrDefaultAsync<AddEmployee>(StoredProcedures.GET_EMP_BY_ID, parameters);
        return result;
    }

    public async Task<int> UpdateEmployee(int id,int managerId,string status,UpdateEmployee employee)
    {
        DynamicParameters parameters = new();
        parameters.Add("@id", id);
        parameters.Add("@FullName",employee.FullName);
        parameters.Add("@UserName",employee.UserName);
        parameters.Add("@Email",employee.Email);
        parameters.Add("@Password",employee.Password);
        parameters.Add("@designationId",employee.DesignationId);
        parameters.Add("@ManagerId",managerId);
        parameters.Add("@Status",status);
        parameters.Add("@Result",dbType:DbType.Int32,direction:ParameterDirection.Output);
        await repository.ExecuteAsync(StoredProcedures.UPDATE_EMPLOYEE,parameters);
        
        var result = parameters.Get<int>("@Result");
        return result;
    }

    public async Task<int> DeleteEmployee(int id)
    {
        DynamicParameters parameters = new();
        parameters.Add("@empId", id);
        parameters.Add("@Result", dbType:DbType.Int32,direction:ParameterDirection.Output);
        await repository.ExecuteAsync(StoredProcedures.DELETE_EMPLOYEE,parameters);
        
        var result = parameters.Get<int>("@Result");
        return result;

    }

    public async Task<int> AddEmployee(AddEmployee employee,string code,string status,int managerId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@FullName",employee.FullName);
        parameters.Add("@UserName",employee.UserName);
        parameters.Add("@Email",employee.Email);
        parameters.Add("@Password",employee.Password);
        parameters.Add("@designationId",employee.DesignationId);
        parameters.Add("@ManagerId",managerId);
        parameters.Add("@RoleId",UMS.Enums.Roles.Employee);
        parameters.Add("@Code",code);
        parameters.Add("@Status",status);
        parameters.Add("@Result",dbType:DbType.Int32,direction:ParameterDirection.Output);
        await repository.ExecuteAsync(StoredProcedures.ADD_EMPLOYEE,parameters);
        var result = parameters.Get<int>("@Result");
        return result;
    }
}