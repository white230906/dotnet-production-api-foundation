# dotnet-production-api-foundation
- Production-ready ASP.NET Core API foundation demonstrating layered architecture,
- Carter endpoints,
- API versioning,
- Problem Details,
- correlation ID,
- Serilog,
- Options pattern,
- domain rules,
- concurrency
- handling, and integration testing.

- Tóm lại cách tư duy của src code này
  -> Src này không có db, bạn chỉ cần clone v bật swagger, -> dự liệu sinh ra khi lên ram
- Được enhance từ 3-tier
=> Cụ thể:
  - Folder Controller trong tầng API cũ được tách ra thành một tầng riêng là Presentation
    - Combine with Carter to minimal module api group
  - Trong tầng Service cũ có 4 class: IService, Service, Request, Response
    - Được tách thành 2 layer riêng: lần lượt là: Application (IService, Service), và Contract (Request, Response)
  - Tầng Domain -> nơi khai báo các antity và business rule