## Task List
1. - [X] Bruno or Scalar? Both.
2. - [X] Switch from Swagger to OpenAPI
2. - [X] Try/Catch - repository or service?




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


