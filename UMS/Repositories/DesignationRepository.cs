using Dapper;
using FluentValidation;
using FluentValidation.Results;
using System.Net;
using UMS.Models.Designation;

namespace UMS.Repositories
{
    public class DesignationRepository(IDapperRepository repository, IValidator<AddDesignationModel> createValidator, IValidator<int> deleteValidator,IValidator<UpdateDesignationModel> updateValidator) : IDesignationRepository
    {
        public async Task<HttpStatusCode> AddDesignation(AddDesignationModel designation)
        {
            ValidationResult validationResult = createValidator.Validate(designation);
            if (!validationResult.IsValid)
                return HttpStatusCode.Unauthorized;
            DynamicParameters parameters = new();
            parameters.Add("@Name", designation.Name);
            parameters.Add("@Description", designation.Description);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await repository.ExecuteAsync(StoredProcedures.ADD_DESIGNATION, parameters);
            
            int result = parameters.Get<int>("@Result");

            if (result == 1) return HttpStatusCode.OK; // Designation added successfully
            
            else if (result == -1) return HttpStatusCode.NotFound; // Designation already exists
           
            else throw new Exception("An error occurred while adding the designation.");
            
        }

        public async Task<HttpStatusCode> DeleteDesignation(int id)
        {
            ValidationResult validationResult = deleteValidator.Validate(id);
            if (!validationResult.IsValid) return HttpStatusCode.Unauthorized;
            DynamicParameters parameters = new();
            parameters.Add("@id", id);
            parameters.Add("@Result",dbType: System.Data.DbType.Int32,direction: System.Data.ParameterDirection.Output);
            await repository.ExecuteAsync(StoredProcedures.DELETE_DESIGNATION, parameters);

            int result = parameters.Get<int>("@Result");
            if (result == 1) return HttpStatusCode.OK;
            if (result == -1) return HttpStatusCode.NotFound;
            else throw new Exception("An error occured while deleting the designation");
        }

        public async Task<IEnumerable<AddDesignationModel>> GetAllDesignations()
        {
            DynamicParameters parameters = new();
            var designation = await repository.QueryAsync<AddDesignationModel>(StoredProcedures.GET_ALL_DESIGNATIONS, parameters);
            return designation ?? [];
        }

        public async Task<AddDesignationModel> GetDesignationById(int id)
        {
            DynamicParameters parameters = new();
            parameters.Add("@designationId", id);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            var designation = await repository.QuerySingleOrDefaultAsync<AddDesignationModel>(StoredProcedures.GET_DESIGNATION_BY_ID,parameters);

            
            return designation ?? new AddDesignationModel()
            {
                Name = "Not Found",
                Description = "Not Found"
            } ;
        }

        public async Task<HttpStatusCode> UpdateDesignation(int id, UpdateDesignationModel designationModel)
        {
            var validation = updateValidator.Validate(designationModel);
            if(!validation.IsValid)
                return HttpStatusCode.BadRequest;
            DynamicParameters parameters = new();
            parameters.Add("@id", id);
            parameters.Add("@Name", designationModel.Name);
            parameters.Add("@Description", designationModel.Description);
            parameters.Add("@Result", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await repository.ExecuteAsync(StoredProcedures.UPDATE_DESIGNATION, parameters);

            var result = parameters.Get<int>("@Result");

            if (result == 1) return HttpStatusCode.OK;
            // Designation added successfully
            else if (result == 0) return HttpStatusCode.Conflict;

            else if (result == -1) return HttpStatusCode.NotFound; // Designation already exists

            else throw new Exception("An error occurred while adding the designation.");


        }
    }
}
