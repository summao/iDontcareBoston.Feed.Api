using iDontCareBoston.Feed.Api;
using iDontCareBoston.Feed.Api.Repositories;
using iDontCareBoston.Feed.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


string connectionString = builder.Configuration["FeedDatabase:ConnectionString"] ?? throw new Exception("FeedDatabase:ConnectionString does not set");
builder.Services.AddSingleton<IMongoClient>(a =>
    new MongoClient(connectionString)
);
builder.Services.AddScoped<IPostService,PostService>();
builder.Services.AddScoped<IPostRepository,PostRepository>();
builder.Services.AddScoped<MongoDbContext>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
