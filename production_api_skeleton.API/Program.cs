var builder = WebApplication.CreateBuilder(args); 
 
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 
 
var app = builder.Build(); 
 
if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
} 
 
app.MapGet("/health", () => 
    Results.Ok(new 
    { 
        Status = "Healthy", 
        Timestamp = DateTimeOffset.UtcNow 
    })); 
 
app.Run(); 
 
// Phục vụ Integration Test bằng WebApplicationFactory sau này. 
public partial class Program 
{ 
    
}