# Database Seeding - FPTU Internal Event

## ?? Thông tin tài kho?n m?c ??nh

### Admin Account
- **Email**: `admin@fptu.edu.vn`
- **Password**: `admin123`
- **Role**: Admin
- **Description**: System administrator with full access to all features and settings

### Roles ?ã ???c t?o s?n:
1. **Admin** - System administrator with full access to all features and settings
2. **Student** - Regular student user who can register for events and view event information
3. **Staff** - Staff member who can manage and organize events
4. **Organizer** - Event organizer with permissions to create and manage events

---

## ?? Cách s? d?ng

### 1. Khi ch?y ?ng d?ng l?n ??u:
Database s? t? ??ng:
- T?o migration
- T?o tables
- Seed roles (Admin, Student, Staff, Organizer)
- Seed admin user

### 2. Login v?i tài kho?n Admin:

**Request:**
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@fptu.edu.vn",
  "password": "admin123"
}
```

**Response:**
```json
{
  "userId": 1,
  "userName": "Administrator",
  "email": "admin@fptu.edu.vn",
  "roleName": "Admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-12-04T10:30:00Z"
}
```

### 3. S? d?ng Token:
Copy token t? response và s? d?ng trong các API calls ti?p theo:
```
Authorization: Bearer {your_token_here}
```

---

## ?? Tùy ch?nh

### Thay ??i thông tin Admin:
Ch?nh s?a file: `Backend_FPTU_Internal_Event.DAL/Data/DbSeeder.cs`

```csharp
private async Task SeedAdminUserAsync()
{
    var adminEmail = "your-email@fptu.edu.vn"; // Thay ??i email
    
    var adminUser = new User
    {
        UserName = "Your Name", // Thay ??i tên
        Email = adminEmail,
        HashPassword = HashPassword("your-password"), // Thay ??i password
        RoleId = adminRole.RoleId,
        Role = adminRole
    };
}
```

### Thêm Roles m?i:
```csharp
private async Task SeedRolesAsync()
{
    var roles = new List<Role>
    {
        // ... existing roles
        new Role
        {
            RoleName = "YourNewRole",
            RoleDescription = "Description for your new role"
        }
    };
}
```

---

## ?? L?u ý

1. **Seeder ch? ch?y 1 l?n**: N?u data ?ã t?n t?i, seeder s? skip
2. **Password ???c hash b?ng SHA256**: Gi?ng v?i logic trong AuthService
3. **Logs**: Check console ?? xem k?t qu? seeding:
   - ? "Roles seeded successfully"
   - ? "Admin user created successfully"
   - ?? "Already exists, skipping..."

---

## ?? Test v?i Swagger

1. M? Swagger: `http://localhost:5000/swagger`
2. Tìm endpoint: `POST /api/auth/login`
3. Click "Try it out"
4. Nh?p:
```json
{
  "email": "admin@fptu.edu.vn",
  "password": "admin123"
}
```
5. Click "Execute"
6. Copy token t? response
7. Click nút "Authorize" ? góc trên
8. Nh?p: `Bearer {token}`
9. Test các protected endpoints

---

## ?? Security Best Practices

?? **QUAN TR?NG:**
- Thay ??i password admin sau l?n ??u login
- Không commit credentials vào Git
- S? d?ng Environment Variables cho production
- Thay ??i JWT SecretKey trong production

---

## ?? Database Structure

```
Roles Table:
??? RoleId (PK)
??? RoleName
??? RoleDescription

Users Table:
??? UserId (PK)
??? UserName
??? Email
??? HashPassword
??? RoleId (FK)
??? Role (Navigation)
```

---

Made with ?? for FPTU Internal Event Management System
