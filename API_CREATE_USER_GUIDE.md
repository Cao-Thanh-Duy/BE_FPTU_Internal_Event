# API T?o User M?i - FPTU Internal Event

## ?? API Endpoint: T?o User M?i (Admin Only)

### **POST** `/api/user`

API này cho phép Admin t?o user m?i trong h? th?ng.

---

## ?? Authorization

**Required**: Admin role only

Header:
```
Authorization: Bearer {admin_token}
```

---

## ?? Request Body

```json
{
  "userName": "string",
  "email": "string",
  "password": "string",
  "roleId": number
}
```

### Validation Rules:

| Field | Type | Required | Rules |
|-------|------|----------|-------|
| userName | string | ? Yes | Min: 3 characters, Max: 100 characters |
| email | string | ? Yes | Valid email format, Max: 150 characters, Must be unique |
| password | string | ? Yes | Min: 6 characters, Max: 100 characters |
| roleId | number | ? Yes | Must be > 0, Must exist in database |

### Available Role IDs:

| RoleId | Role Name | Description |
|--------|-----------|-------------|
| 1 | Admin | System administrator with full access |
| 2 | Student | Regular student user |
| 3 | Staff | Staff member who can manage events |
| 4 | Organizer | Event organizer |

---

## ? Success Response

**Status Code**: `201 Created`

**Headers**:
```
Location: /api/user/{userId}
```

**Body**:
```json
{
  "success": true,
  "message": "User created successfully",
  "data": {
    "userId": 2,
    "userName": "Nguyen Van A",
    "email": "student1@fpt.edu.vn",
    "roleName": "Student"
  }
}
```

---

## ? Error Responses

### 1. Validation Error (400 Bad Request)

```json
{
  "success": false,
  "message": "Invalid input",
  "errors": [
    "Username is required",
    "Email must be a valid email address",
    "Password must be at least 6 characters"
  ]
}
```

### 2. Duplicate Email (400 Bad Request)

```json
{
  "success": false,
  "message": "Email already exists or RoleId is invalid"
}
```

### 3. Unauthorized (401 Unauthorized)

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 4. Forbidden (403 Forbidden)

N?u user không ph?i Admin:
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Forbidden",
  "status": 403
}
```

### 5. Internal Server Error (500)

```json
{
  "success": false,
  "message": "Internal server error",
  "detail": "Error details..."
}
```

---

## ?? Testing v?i Swagger

### B??c 1: Login v?i Admin
```
POST /api/auth/login

{
  "email": "admin@fptu.edu.vn",
  "password": "admin123"
}
```

### B??c 2: Copy Token
Copy `token` t? response

### B??c 3: Authorize
1. Click nút "Authorize" trong Swagger
2. Nh?p: `Bearer {your_token}`
3. Click "Authorize"

### B??c 4: Test API
1. Tìm endpoint `POST /api/user`
2. Click "Try it out"
3. Nh?p request body:
```json
{
  "userName": "Test Student",
  "email": "test.student@fpt.edu.vn",
  "password": "password123",
  "roleId": 2
}
```
4. Click "Execute"

---

## ?? Testing v?i Postman/cURL

### Example Request:

```bash
curl -X POST "http://localhost:5000/api/user" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "userName": "Test Student",
    "email": "test.student@fpt.edu.vn",
    "password": "password123",
    "roleId": 2
  }'
```

---

## ?? Related APIs

### Get All Users (Admin only)
```
GET /api/user
Authorization: Bearer {admin_token}
```

Response:
```json
{
  "success": true,
  "message": "Users retrieved successfully",
  "data": [
    {
      "userId": 1,
      "userName": "Administrator",
      "email": "admin@fptu.edu.vn",
      "roleName": "Admin"
    },
    {
      "userId": 2,
      "userName": "Test Student",
      "email": "test.student@fpt.edu.vn",
      "roleName": "Student"
    }
  ]
}
```

### Get User by ID (Admin only)
```
GET /api/user/{id}
Authorization: Bearer {admin_token}
```

Response:
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "userId": 2,
    "userName": "Test Student",
    "email": "test.student@fpt.edu.vn",
    "roleName": "Student"
  }
}
```

---

## ?? Security Notes

1. **Password Hashing**: Passwords ???c hash b?ng SHA256 tr??c khi l?u vào database
2. **Authorization**: Ch? users v?i role "Admin" m?i có th? t?o users m?i
3. **Email Uniqueness**: System t? ??ng check email trùng l?p
4. **JWT Token**: Token expires sau 60 phút (có th? config trong appsettings.json)

---

## ?? Tips

1. **Testing**: Nên t?o test users v?i các roles khác nhau ?? test authorization
2. **Email Format**: Khuy?n ngh? s? d?ng email có domain @fpt.edu.vn ho?c @fptu.edu.vn
3. **Password**: Trong production, nên yêu c?u password ph?c t?p h?n (uppercase, lowercase, s?, ký t? ??c bi?t)
4. **RoleId**: Luôn ki?m tra RoleId có t?n t?i tr??c khi g?i API

---

## ?? Example Use Cases

### 1. T?o Student User
```json
{
  "userName": "Nguyen Van A",
  "email": "anvn@fpt.edu.vn",
  "password": "student123",
  "roleId": 2
}
```

### 2. T?o Staff User
```json
{
  "userName": "Tran Thi B",
  "email": "btt@fptu.edu.vn",
  "password": "staff123",
  "roleId": 3
}
```

### 3. T?o Organizer User
```json
{
  "userName": "Le Van C",
  "email": "clv@fptu.edu.vn",
  "password": "organizer123",
  "roleId": 4
}
```

---

Made with ?? for FPTU Internal Event Management System
