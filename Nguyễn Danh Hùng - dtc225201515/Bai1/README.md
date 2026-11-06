# EduSubmit dtc225201515

Bài 1 học phần Phát triển ứng dụng phân tán của sinh viên Nguyễn Danh Hùng, MSSV `dtc225201515`.

## Cấu trúc

- `src/Submission.Api`: ASP.NET Core Minimal API.
- `src/Submission.Client`: ứng dụng console gọi API qua HTTP.
- `requests.http`: các request kiểm thử 200, 201, 400 và 404.
- `docs/bai1.md`: nhật ký thực hành và ảnh minh chứng.

## Chạy bài

Mở terminal thứ nhất:

```powershell
dotnet run --project src/Submission.Api --urls http://localhost:5000
```

Mở terminal thứ hai:

```powershell
dotnet run --project src/Submission.Client -- http://localhost:5000
```

API cung cấp các endpoint:

| Method | Endpoint | Kết quả |
| --- | --- | --- |
| GET | `/` | Trạng thái dịch vụ |
| GET | `/api/assignments` | Danh sách bài tập |
| POST | `/api/submissions` | Tạo lượt nộp |
| GET | `/api/submissions/{id}` | Đọc lại lượt nộp hoặc trả 404 |
