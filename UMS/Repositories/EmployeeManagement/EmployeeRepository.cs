using System.Net;
using Dapper;
using UMS.DynamicParametersExtension;
using UMS.Models.Employee;
using UMS.Models.Entities;

namespace UMS.Repositories.EmployeeManagement;

public class EmployeeRepository(IDapperRepository repository) : IEmployeeRepository
{
    public List<Employee> GetAllEmployees(DataTableRequest request)
    {
        DynamicParameters parameters = new();
        parameters.AddDefaultPaginationParameters(request);
        var result = repository.Query<Employee>(StoredProcedures.GET_ALL_EMPLOYEE, parameters);
        return result.ToList();
    }

    public Employee GetEmployeeById(int id)
    {
        throw new NotImplementedException();
    }

    public (HttpStatusCode, bool) UpdateEmployee(AddEmployee employee)
    {
        throw new NotImplementedException();
    }

    public (HttpStatusCode, bool) DeleteEmployee(int id)
    {
        throw new NotImplementedException();
    }

    public (HttpStatusCode, bool) AddEmployee(AddEmployee employee)
    {
        throw new NotImplementedException();
    }
}