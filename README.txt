1. Branching
- Dev tạo nhánh riêng (clone từ nhánh develop) để viết tính năng nhận
- Thực hiện request merge vào nhánh develop theo format: features/<Tên sử dụng>/<Mô tả tính năng>
- Các dev review chéo lẫn nhau
==========================================================================================================
2. Tạo & Cập nhật thông tin DB
- Cài đặt Microsoft.EntityFrameworkCore.Tools => để sử dụng được các câu lệnh Add-Migration, Remove-Migration...
- Cài đặt Microsoft.EntityFrameworkCore.Design => design DB (code first) và cập nhật trên môi trường dev (localhost) (Gồm cả Project liên quan).
- Sau khi thay đổi schema, dev cần thực hiện các bước sau:
	+ Để Project chạy là Startup Project => Chuỗi kết nối SQL được đặt tại đây.
	+ Tạo class kế implement từ IDesignTimeDbContextFactory và đặt tại lớp Data của dự án. Class này sẽ dựa trên Startup Project để xác định file appsetting.json để đọc chuỗi kết nối DB.
	+ Mở cửa sổ Package Manager Console - Chọn Default Project là project data và gõ các lệnh migration - Update-Database.
==========================================================================================================
3. Tạo IssuerSigningKey
- Sử dụng thư viện StringHelper.Hs256Signature(). 