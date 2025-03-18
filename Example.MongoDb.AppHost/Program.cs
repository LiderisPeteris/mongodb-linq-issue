var builder = DistributedApplication.CreateBuilder(args);

var mongoPassword = builder.AddParameter("password", "admin");
var mongoUsername = builder.AddParameter("user", "admin");

var mongoDb = builder.AddMongoDB("mongodb", 27017, mongoUsername, mongoPassword)
    .WithDataVolume("mongo")
    .AddDatabase("ExampleDb");

var consoleApp = builder.AddProject<Projects.Example_App>("console-app")
                        .WithReference(mongoDb)
                        .WaitFor(mongoDb);

builder.Build().Run();
