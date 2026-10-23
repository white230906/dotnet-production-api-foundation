using production_api_skeleton.API.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
 
var app = builder.Build(); 

app.UseApiPipeline();
 
app.Run(); 
 
// Cho phép WebApplicationFactory truy cập khi viết integration test.public partial class Program 
public partial class Program 

{ 
    
}