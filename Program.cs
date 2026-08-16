using System.Text;

using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.PeliculasMappers;
using ApiPeliculas.Repositories;
using ApiPeliculas.Repositories.IRepositories;

using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionLocalSql"));
});


//.NET Identity Authentication config Debe ir antes de jwt
builder.Services.AddIdentity<UsuarioIdentity, IdentityRole>().AddEntityFrameworkStores<AplicationDBContext>();

//JWT Authentication config

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddResponseCaching();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddAutoMapper(map => map.AddMaps(typeof(PeliculasMapper)));

builder.Services.AddControllers(options =>
{
    //Configurar Cache global
    options.CacheProfiles.Add("Global30Cache", new CacheProfile() { Duration = 30});
});


//Configuracion para versionamiento
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    /*options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version")//?api-version=1.0
        //new HeaderApiVersionReader("X-Version"),
        //new MediaTypeApiVersionReader("ver"));
    );*/
});

apiVersioningBuilder.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    }
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Description = "For JWT authentication. \r\n\r\n " +
        "Add the word 'Bearer'  followed by a space and your token \r\n\r\n" +
        "Example: \"Bearer ey5i43nrjakd8sa6kfd...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        { 
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Movies API V1",
        Description = "Movies API",
        TermsOfService = new Uri("https://github.com/Meneses-11/ApiPeliculas"),
        Contact = new OpenApiContact
        {
            Name = "Adrian Meneses",
            Url = new Uri("https://www.linkedin.com/in/menesesadrian/")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://github.com/Meneses-11/ApiPeliculas.git")
        }
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Movies API V2",
        Description = "Movies API",
        TermsOfService = new Uri("https://github.com/Meneses-11/ApiPeliculas"),
        Contact = new OpenApiContact
        {
            Name = "Adrian Meneses",
            Url = new Uri("https://www.linkedin.com/in/menesesadrian/")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            Url = new Uri("https://github.com/Meneses-11/ApiPeliculas.git")
        }
    });
});

string[] allowedOriginsDev = builder.Configuration.GetSection("ApiSettings:AllowedOriginsDev").Get<string[]>() ?? [];
string[] allowedOrigins = builder.Configuration.GetSection("ApiSettings:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => {
    //Cors para ambiente de produccion
    options.AddPolicy("CORSPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
    //Cors para ambiente de desarrollo
    options.AddPolicy("CORSDev", policy =>
    {
        policy.WithOrigins(allowedOriginsDev)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
});
if (app.Environment.IsDevelopment())
{
    app.UseCors("CORSDev");
}
else
{
    app.UseCors("CORSPolicy");
}

//Statics file Config
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
