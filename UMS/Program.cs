using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UMS;
using UMS.Data;
using UMS.Encryption;
using UMS.Middlewares;
using UMS.Models;
using UMS.Models.Designation;
using UMS.Models.Employee;
using UMS.Models.Manager;
using UMS.Repositories;
using UMS.Repositories.AttendanceRepo;
using UMS.Repositories.BlackListToken;
using UMS.Repositories.EmployeeManagement;
using UMS.Repositories.ManagerManagement;
using UMS.Services;
using UMS.Services.Attendance;
using UMS.Validations.Account.Employee;
using UMS.Validations.Account.Manager;
using UMS.Validations.Deisgnation;
using UMS.Validations.ManagerValidator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "UMS_"; // Optional prefix for cache keys
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1); 
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V"; 
        options.SubstituteApiVersionInUrl = true;
    })
    .AddMvc();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IManagerAttendanceRepository,ManagerAttendanceRepository>();
builder.Services.AddScoped<ManagerAttendanceService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IDapperRepository, DapperRepository>();
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
builder.Services.AddScoped<BlackListTokenService>();
builder.Services.AddScoped<IBlackListTokenRepository,BlackListTokenRepository>();
builder.Services.AddScoped<DesignationRepository>();
builder.Services.AddScoped<IValidator<AddEmployee>,EmployeeRegisterValidation>();
builder.Services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
builder.Services.AddScoped<ManagerService>();
builder.Services.AddScoped<IValidator<AddEmployee>, CreateEmployeeValidation>();
builder.Services.AddScoped<IValidator<UpdateEmployee>, UpdateEmployeeValidator>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<IValidator<AddDesignationModel>, AddDesignationValidator>();
builder.Services.AddScoped<IValidator<int>, DeleteDesignationValidator>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<AesEncryption>();
builder.Services.AddScoped<IValidator<AddManager>, CreateManagerValidator>();
builder.Services.AddScoped<IValidator<ManagerRegisterModel>, ManagerRegisterValidator>();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Enter Access Token",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme,Array.Empty<string>()}
    });
});

var key = builder.Configuration["JWTConfig:Key"];
if (string.IsNullOrEmpty(key))
{
    throw new ArgumentException("JWT configuration key is not set.");
}
builder.Services.Configure<EmailSettingsModel>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTConfig:Issuer"],
        ValidAudience = builder.Configuration["JWTConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key))
    };

}
);
builder.Services.AddSwaggerGen(c =>
{

    c.AddSecurityDefinition("X-Signature", new OpenApiSecurityScheme
    {
        Name = "X-Signature",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "Custom signature for securing API calls"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Signature"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;                 // Security best practice
    options.Cookie.IsEssential = true;              // Required for GDPR compliance
});


builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IValidator<UpdateDesignationModel>, UpdateDesignationValidator>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url,name);
        }
    });
}

app.UseMiddleware<BlackListTokenMiddleware>();
app.UseHttpsRedirection();

app.UseSession();

app.UseAuthorization();

app.MapControllers();

app.Run();
