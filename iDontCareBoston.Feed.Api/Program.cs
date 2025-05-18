using iDontCareBoston.Feed.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.Configure<FeedDatabaseSetting>(
// builder.Configuration.GetSection("FeedDatabase"));

// Add services to the container.
builder.Services.AddSingleton<IMongoClient>(a =>
    new MongoClient("mongodb://mongo:mongo@127.0.0.1:27018/admin?retryWrites=true&loadBalanced=false&connectTimeoutMS=10000&authSource=admin&authMechanism=SCRAM-SHA-1")
);
builder.Services.AddScoped<PostService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
