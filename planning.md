## Task List

1. - [x] Bruno or Scalar? Both.
2. - [x] Switch from Swagger to OpenAPI
3. - [ ] Try/Catch - repository or service layer?
4. - [x] Global Exception Handler
5. - [ ] Logging
6. - [ ] File size limitations - to prevent overload?
7. - [ ] Dedicated validation class for File/Folder validation?
8. - [ ] Add ModelState validation to relevant endpoints.
9. - [x] Add empty constructors to entities

FileService - improvements

- Validate file type

## Implementing Identity Core

Phase 1: Authentication Setup

Create JwtSettings configuration in appsettings.json
Create AuthDTO classes for registration and login
Create a TokenService to generate JWT tokens
Create an AuthController with Register and Login endpoints
Test user registration and login

Phase 2: Securing Resources

Configure JWT authentication middleware
Add authorization to existing controllers
Update services to filter resources by the current user
Test accessing protected resources with tokens

Phase 3: User Management (if time permits)

Implement password reset
Implement user profile management
Add role-based authorization (if needed)
