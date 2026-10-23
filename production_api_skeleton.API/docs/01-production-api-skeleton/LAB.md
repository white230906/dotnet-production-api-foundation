# Mini-project 01: Production API Skeleton

## 1. Mục tiêu

Tạo một ASP.NET Core 8 API nhỏ để học cách tổ chức host và HTTP layer theo phong cách của `buildx-be`, bắt đầu từ nền tảng 3-tier quen thuộc.

Technical core của lab:

- `Program.cs` chỉ giữ vai trò composition root.
- Đăng ký dependency bằng extension methods.
- Dùng Carter để chia endpoint thành module thay cho Controller.
- Dùng Options pattern và kiểm tra cấu hình khi startup.
- Xây middleware pipeline có thứ tự rõ ràng.
- Chuẩn hóa lỗi bằng Problem Details.
- API versioning, Swagger và structured logging.
- Viết integration test tối thiểu cho HTTP pipeline.

Sau lab này, bạn phải nhìn vào `Program.cs` của `buildx-be` và hiểu được cách host ghép các module kỹ thuật lại với nhau.

## 2. Production motivation

Các cơ chế của lab được lấy cảm hứng từ những phần đã xác minh trong production source:

- Composition root và đăng ký Carter: `buildx-be/src/bmt-be.api/Program.cs`.
- Carter endpoint modules: `buildx-be/src/bmt-be.presentation/apis`.
- Middleware pipeline: `buildx-be/src/bmt-be.api/dependencyInjection/extensions/RequestProtectionExtensions.cs`.
- Global exception handling: `buildx-be/src/bmt-be.api/middleware/ExceptionHandlingMiddleware.cs`.
- API versioning và Swagger: `buildx-be/src/bmt-be.api/dependencyInjection/extensions/SwaggerExtensions.cs`.

Không sao chép nghiệp vụ hoặc implementation của production. Chỉ tái tạo cách tổ chức và áp lực kỹ thuật.

## 3. Source dùng làm điểm xuất phát

Code base quen thuộc:

```text
D:\dev\csharp\projects\code-base-3-tier-architecture
```

Không sửa trực tiếp repository này. Tạo một source mới cho lab và dùng code base trên để tham khảo hoặc chuyển từng phần cần thiết.

Được phép tham khảo:

- `CodeBase.API/Program.cs`.
- `CodeBase.API/Controller/WeatherForecastController.cs`.
- `CodeBase.API/Middleware/GlobalExceptionHandlerMiddleware.cs`.
- Các extension Swagger, CORS và rate limiting hiện có.

Không mang sang Lab 1:

- `.git`, `.idea`, `bin`, `obj`.
- Entity, DbContext và migration.
- JWT, Redis, Quartz, Cloudinary và MailKit.
- Các package không được sử dụng trong Lab 1.

Mục tiêu là refactor tư duy từ Controller 3-tier, không phải sao chép toàn bộ template cũ.

## 4. Bài toán tối thiểu

Xây một API quản lý `WorkItem` trong bộ nhớ.

Một `WorkItem` gồm:

```text
Id          Guid
Title       string
Status      Pending | InProgress | Completed
CreatedAt   DateTimeOffset
```

Quy tắc tối thiểu:

- `Title` bắt buộc, sau khi trim dài từ 3 đến giới hạn cấu hình.
- Work item mới luôn có trạng thái `Pending`.
- Không tìm thấy work item trả `404` theo Problem Details.
- Input không hợp lệ trả `422` theo Problem Details.
- Lỗi không dự kiến trả `500` nhưng không lộ stack trace.
- Dữ liệu chỉ tồn tại trong bộ nhớ và mất khi restart ứng dụng.

## 5. API contract

Triển khai các endpoint sau:

| Method | Route | Kết quả |
|---|---|---|
| `GET` | `/api/v1/work-items` | Danh sách work item |
| `GET` | `/api/v1/work-items/{id}` | Một work item hoặc `404` |
| `POST` | `/api/v1/work-items` | Tạo mới và trả `201` |
| `PUT` | `/api/v1/work-items/{id}/status` | Đổi trạng thái hoặc `404`/`422` |
| `GET` | `/health` | Trả trạng thái API |

Request/response chỉ cần đủ cho bài tập. Không thêm paging, authentication hoặc database.

## 6. Technical constraints

- .NET 8.
- Nullable reference types bật.
- Implicit usings bật.
- Carter `8.1.0`.
- Asp.Versioning `8.1.0`.
- Serilog.AspNetCore `8.0.2`.
- Swashbuckle.AspNetCore `8.1.1`.
- Không dùng MVC Controller cho endpoint nghiệp vụ.
- Không khai báo endpoint nghiệp vụ trực tiếp trong `Program.cs`.
- Không dùng MediatR trong Lab 1; đó là technical core của Lab 2.
- Không dùng database, repository hoặc UnitOfWork.
- Không dùng authentication/authorization.
- Không copy business code từ `buildx-be`.
- Mọi dependency phải được đăng ký tại composition root hoặc qua extension được gọi từ composition root.

## 7. Cấu trúc đích

```text
01-production-api-skeleton/
├── ProductionApiSkeleton.sln
├── README.md
├── src/
│   ├── Lab1.Api/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── DependencyInjection/
│   │   │   ├── ApiServiceCollectionExtensions.cs
│   │   │   └── ApiApplicationBuilderExtensions.cs
│   │   ├── Middleware/
│   │   │   ├── CorrelationIdMiddleware.cs
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   └── Options/
│   │       └── WorkItemOptions.cs
│   ├── Lab1.Presentation/
│   │   ├── Apis/
│   │   │   └── WorkItems/
│   │   │       └── WorkItemApi.cs
│   │   └── Abstractions/
│   │       └── ApiEndpoint.cs
│   ├── Lab1.Application/
│   │   └── WorkItems/
│   │       ├── IWorkItemService.cs
│   │       └── InMemoryWorkItemService.cs
│   ├── Lab1.Domain/
│   │   └── WorkItems/
│   │       ├── WorkItem.cs
│   │       └── WorkItemStatus.cs
│   └── Lab1.Contract/
│       └── WorkItems/
│           ├── CreateWorkItemRequest.cs
│           ├── UpdateWorkItemStatusRequest.cs
│           └── WorkItemResponse.cs
└── tests/
    └── Lab1.Api.IntegrationTests/
```

Dependency direction:

```text
Lab1.Api
├── Lab1.Presentation
└── Lab1.Application

Lab1.Presentation
├── Lab1.Application
└── Lab1.Contract

Lab1.Application
├── Lab1.Domain
└── Lab1.Contract

Lab1.Domain       → không tham chiếu project khác
Lab1.Contract     → không tham chiếu project khác
```

Không cho `Domain` hoặc `Application` tham chiếu `Api`/`Presentation`.

## 8. Khởi tạo solution

Tạo source mới, không tạo bên trong production repository:

```powershell
New-Item -ItemType Directory -Path "D:\dev\csharp\projects\backend-learning\01-production-api-skeleton\src" -Force
New-Item -ItemType Directory -Path "D:\dev\csharp\projects\backend-learning\01-production-api-skeleton\tests" -Force
Set-Location "D:\dev\csharp\projects\backend-learning\01-production-api-skeleton"

dotnet new sln -n ProductionApiSkeleton
dotnet new webapi -n Lab1.Api -o src/Lab1.Api --use-controllers false --framework net8.0
dotnet new classlib -n Lab1.Presentation -o src/Lab1.Presentation --framework net8.0
dotnet new classlib -n Lab1.Application -o src/Lab1.Application --framework net8.0
dotnet new classlib -n Lab1.Domain -o src/Lab1.Domain --framework net8.0
dotnet new classlib -n Lab1.Contract -o src/Lab1.Contract --framework net8.0
dotnet new xunit -n Lab1.Api.IntegrationTests -o tests/Lab1.Api.IntegrationTests --framework net8.0

dotnet sln add src/Lab1.Api/Lab1.Api.csproj
dotnet sln add src/Lab1.Presentation/Lab1.Presentation.csproj
dotnet sln add src/Lab1.Application/Lab1.Application.csproj
dotnet sln add src/Lab1.Domain/Lab1.Domain.csproj
dotnet sln add src/Lab1.Contract/Lab1.Contract.csproj
dotnet sln add tests/Lab1.Api.IntegrationTests/Lab1.Api.IntegrationTests.csproj
```

Thêm project references theo dependency direction ở mục 7. Tự viết các lệnh `dotnet add ... reference ...`, sau đó dùng lệnh sau để kiểm tra:

```powershell
dotnet list ProductionApiSkeleton.sln reference
```

Thêm package đúng project:

- `Lab1.Api`: Carter, API versioning, Swagger, Serilog.AspNetCore.
- `Lab1.Presentation`: Carter và API versioning abstractions cần cho route module.
- `Lab1.Api.IntegrationTests`: `Microsoft.AspNetCore.Mvc.Testing` phiên bản .NET 8 phù hợp.

Sau khi thêm package:

```powershell
dotnet restore
dotnet build
```

Không tiếp tục checkpoint sau nếu solution chưa build thành công.

## 9. Checkpoint 1 — Làm sạch template và khóa dependency

Việc cần làm:

1. Xóa WeatherForecast endpoint sinh tự động.
2. Xóa các `Class1.cs`.
3. Thêm project references đúng sơ đồ.
4. Kiểm tra không có dependency ngược.
5. Bật `Nullable` và `ImplicitUsings` cho tất cả project.
6. Thêm `public partial class Program;` để integration test có thể tạo test host.

Xác minh:

```powershell
dotnet build
dotnet list ProductionApiSkeleton.sln reference
```

Điều kiện qua checkpoint:

- Solution build không lỗi.
- Không còn WeatherForecast.
- `Domain` và `Contract` không tham chiếu project nào.

## 10. Checkpoint 2 — Domain, contract và application service

Việc cần làm:

1. Tạo `WorkItem` và `WorkItemStatus` trong Domain.
2. Tạo request/response records trong Contract.
3. Định nghĩa `IWorkItemService` trong Application.
4. Viết `InMemoryWorkItemService` bằng collection thread-safe.
5. Đăng ký service với lifetime phù hợp để dữ liệu không mất sau từng request.

Behavior contract của service:

```text
GetAll()                         → trả snapshot danh sách
GetById(id)                     → trả item hoặc not-found result
Create(title)                   → trim, kiểm tra title, tạo Pending
UpdateStatus(id, newStatus)     → cập nhật hoặc not-found/validation result
```

Không throw exception cho lỗi nghiệp vụ dự kiến như title sai hoặc không tìm thấy. Bạn tự chọn một Result model nhỏ hoặc một kiểu trả về rõ ràng.

Câu hỏi phải trả lời trước khi qua checkpoint:

- Vì sao service không nên đăng ký `Scoped` nếu collection nằm trực tiếp bên trong instance?
- `Singleton` service chứa state mutable cần bảo vệ concurrency như thế nào?

Xác minh:

- Build thành công.
- Tạo hai work item từ hai scope khác nhau vẫn đọc được cùng dữ liệu.
- Hai request tạo đồng thời không làm hỏng collection.

## 11. Checkpoint 3 — Carter endpoint module

Việc cần làm:

1. Tạo `WorkItemApi : ICarterModule` trong Presentation.
2. Khai báo toàn bộ route trong `AddRoutes(IEndpointRouteBuilder app)`.
3. Gom endpoint vào một route group `/api/v{version:apiVersion}/work-items`.
4. Inject `IWorkItemService` trực tiếp vào endpoint handler.
5. Handler chỉ bind input, gọi Application và ánh xạ kết quả sang HTTP.
6. Thêm metadata: name, summary, produces và status code.
7. Đăng ký Carter trong DI và gọi `MapCarter()` khi map endpoint.

Không được:

- Tạo `WorkItemsController`.
- Đặt business rule trong handler.
- Truy cập collection trực tiếp từ Presentation.
- Viết `app.MapGet("/api/v1/work-items", ...)` trực tiếp trong `Program.cs`.

Xác minh thủ công:

```text
POST /api/v1/work-items
GET  /api/v1/work-items
GET  /api/v1/work-items/{id}
PUT  /api/v1/work-items/{id}/status
```

Điều kiện qua checkpoint:

- Mọi endpoint nghiệp vụ nằm trong Carter module.
- `Program.cs` không biết chi tiết route của WorkItem.
- Có thể giải thích Carter phát hiện và map module như thế nào.

## 12. Checkpoint 4 — Options và startup validation

Tạo section cấu hình:

```json
{
  "WorkItems": {
    "MaxTitleLength": 100,
    "DefaultPageSize": 20
  }
}
```

Yêu cầu:

1. Bind section vào `WorkItemOptions`.
2. Validate `MaxTitleLength` nằm trong một khoảng hợp lý.
3. Validate `DefaultPageSize` lớn hơn 0.
4. Dùng `ValidateOnStart()`.
5. Service nhận option qua `IOptions<WorkItemOptions>` hoặc `IOptionsMonitor<WorkItemOptions>`; chọn một loại và ghi lý do trong README.

Failure experiment bắt buộc:

- Đặt `MaxTitleLength` thành giá trị không hợp lệ.
- Chạy ứng dụng.
- Xác nhận ứng dụng từ chối startup thay vì đợi đến request đầu tiên.

Điều kiện qua checkpoint:

- Cấu hình sai làm startup thất bại với thông báo đủ nhận diện section/property.
- Không đọc trực tiếp `IConfiguration` trong WorkItem service.

## 13. Checkpoint 5 — Middleware và Problem Details

Tạo hai middleware:

### Correlation ID

- Đọc `X-Correlation-ID` từ request nếu hợp lệ; nếu không có thì tạo mới.
- Đưa correlation ID vào response header.
- Đưa giá trị vào logging scope.
- Không tin một header dài hoặc chứa ký tự không hợp lệ.

### Global exception handling

- Catch exception chưa được xử lý.
- Nếu response đã bắt đầu, không cố ghi response mới.
- Log exception cùng correlation ID.
- Trả `application/problem+json`.
- Development có thể có thông tin hỗ trợ debug vừa đủ.
- Non-development không trả stack trace hoặc internal exception message.

Chuẩn lỗi tối thiểu:

| Tình huống | Status |
|---|---:|
| Input/domain validation | `422` |
| Không tìm thấy resource | `404` |
| Exception không dự kiến | `500` |

Tự quyết định middleware order và ghi chú lý do trong README. Correlation ID phải xuất hiện cả ở response lỗi.

Failure experiment bắt buộc:

- Tạo tạm một test-only endpoint hoặc dependency ném exception.
- Xác minh `500` theo Problem Details.
- Xác minh response không chứa stack trace ở Production environment.
- Xóa test-only endpoint nếu không được giới hạn cho test environment.

## 14. Checkpoint 6 — API versioning, Swagger và logging

Yêu cầu:

1. Bật URL-segment API versioning.
2. Đặt API version mặc định là v1.
3. Swagger hiển thị document v1.
4. Carter route group khai báo hỗ trợ v1.
5. Cấu hình Serilog từ `appsettings.json`.
6. Bật request logging với path, method, status code và elapsed time.
7. Không log request body.
8. `/health` không cần đi qua business service.

Xác minh:

- `/api/v1/work-items` hoạt động.
- Version không hỗ trợ trả lỗi thích hợp.
- Swagger hiển thị đủ bốn WorkItem endpoints.
- Log có correlation ID cho request bình thường và request lỗi.

## 15. Checkpoint 7 — Làm gọn composition root

Mục tiêu cuối cùng của `Program.cs` là chỉ biểu diễn thứ tự lắp ráp hệ thống, ví dụ về mặt ý nghĩa:

```text
Create builder
→ Add configuration/logging
→ Add API services
→ Add application services
→ Build
→ Use API middleware pipeline
→ Map Carter modules and health endpoint
→ Run
```

Tách đăng ký vào extension methods nhưng không tạo extension chỉ để giấu một dòng code. Nhóm theo trách nhiệm:

- API hosting và versioning.
- Application services.
- Middleware pipeline.
- Endpoint mapping.

Điều kiện qua checkpoint:

- Đọc `Program.cs` có thể thấy ngay startup flow.
- Không còn implementation detail của WorkItem trong `Program.cs`.
- Không có service locator hoặc `BuildServiceProvider()` trong lúc đăng ký DI.

## 16. Required integration tests

Viết bằng `WebApplicationFactory<Program>`.

Tối thiểu phải có:

1. `POST` hợp lệ trả `201` và response có `Location`.
2. `POST` title rỗng hoặc quá dài trả `422` Problem Details.
3. `GET` ID không tồn tại trả `404` Problem Details.
4. Exception không dự kiến trả `500` và không lộ stack trace ở Production.
5. Request có `X-Correlation-ID` hợp lệ thì response trả lại cùng giá trị.
6. Request không có correlation ID thì response được cấp một giá trị mới.
7. Route v1 hoạt động; version không hỗ trợ không bị xử lý như v1.
8. Gửi nhiều request tạo work item song song; tất cả ID phải duy nhất và service không lỗi concurrency.

Không mock Carter hoặc middleware. Test qua HTTP pipeline thật trong test server.

## 17. Definition of done

Lab hoàn thành khi:

- `dotnet build` thành công, không warning do code của lab.
- `dotnet test` thành công.
- Không có Controller.
- Không có endpoint nghiệp vụ trong `Program.cs`.
- Carter module chứa HTTP concerns, Application chứa use case, Domain chứa model, Contract chứa request/response.
- Options được validate khi startup.
- Lỗi được trả theo Problem Details nhất quán.
- Correlation ID có trong response và log.
- Swagger/API versioning hoạt động.
- README ghi được request lifecycle và lý do của middleware order.
- Bạn tự thêm được một endpoint `DELETE /api/v1/work-items/{id}` mà không sửa `Program.cs`.

## 18. Proof of understanding

Trước khi yêu cầu review, tự trả lời trong `README.md`:

1. `AddCarter()` và `MapCarter()` khác nhau ở đâu?
2. Carter khác Controller ở cách tổ chức nào, nhưng vẫn dùng chung ASP.NET Core pipeline ra sao?
3. Vì sao `Program.cs` được gọi là composition root?
4. Vì sao middleware order làm thay đổi behavior?
5. Tại sao exception middleware phải kiểm tra `Response.HasStarted`?
6. `ValidateOnStart()` giải quyết failure mode nào?
7. Vì sao Domain không được tham chiếu Presentation/API?
8. Vì sao in-memory store là Singleton trong lab này, và rủi ro của mutable Singleton là gì?
9. Nếu thay in-memory store bằng PostgreSQL thì project/layer mới nào sẽ xuất hiện?
10. Điều gì xảy ra nếu endpoint chứa toàn bộ validation và business rule?

## 19. So sánh lại với production sau khi hoàn thành

Sau khi lab chạy và test pass, đối chiếu:

1. `Lab1.Api/Program.cs` với `buildx-be/src/bmt-be.api/Program.cs`.
2. `WorkItemApi` với một Carter module nhỏ trong `bmt-be.presentation/apis`.
3. Middleware order của lab với `UseApiPipeline` trong production.
4. Problem Details của lab với `ExceptionHandlingMiddleware` và `ApiEndpoint` trong production.
5. API versioning/Swagger registration của lab với extension tương ứng trong production.

Chỉ so sánh các cơ chế đã học; chưa đi vào MediatR, EF Core, authentication hoặc messaging.

## 20. Cách làm việc với mentor

Thực hiện từng checkpoint theo thứ tự. Khi gặp phần chưa hiểu, gửi:

```text
Lab 1 - Checkpoint <số>
Tôi đang làm: <việc đang làm>
Tôi chưa hiểu: <khái niệm hoặc đoạn code>
Code/error hiện tại: <đường dẫn hoặc lỗi>
```

Khi hoàn thành một checkpoint, yêu cầu review checkpoint đó trước khi chuyển tiếp nếu bạn chưa chắc dependency direction hoặc runtime behavior.

Không yêu cầu lời giải toàn bộ lab. Ưu tiên hỏi cơ chế, failure mode và lý do thiết kế.
