# API_Products

WEB API ระบบจัดการสินค้า CRUD (สร้าง,ดู,แก้ไข,ลบ) โดยใช้
- EF Core
- Data Transfer Objects (DTOs)
- แยกชั้น Interface, Service, Controller

## RUN

ติดตั้ง EF Core และ สร้างฐานข้อมูล

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/API_Products
```

รัน WEB API

```bash
dotnet run --project src/API_Products
```
ทดสอบ WEB API ที่ URL `http://localhost:5055/scalar/v1`

## Endpoints
|Method|URL|ความหมาย|
|---|---|---|
|GET|`api/Products`|ดูสินค้าทั้งหมด|
|GET|`api/Products/{id}`|ดูสินค้าตามรหัสสินค้า|
|GET|`api/Products/search?name=`|ดูสินค้าตามชื่อสินค้า|
|GET|`api/Products/expensive?minPrice=`|ดูสินค้าตามราคาหรือมากกว่า|
|POST|`api/Products`|เพิ่มสินค้า|
|PUT|`api/Products/{id}`|แก้ไขสินค้า|
|DELETE|`api/Products/{id}`|ลบสินค้าตามรหัสสินค้า|
|DELETE|`api/Products`|ลบสินค้าทั้งหมด|
