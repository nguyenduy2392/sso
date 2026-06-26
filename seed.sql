-- ============================================================
-- SSO Seed Data
-- Passwords: Admin@123 / User@123
-- ============================================================

-- Tenants
DECLARE @TenantId1 UNIQUEIDENTIFIER = 'A1000000-0000-0000-0000-000000000001'
DECLARE @TenantId2 UNIQUEIDENTIFIER = 'A2000000-0000-0000-0000-000000000002'

INSERT INTO Tenants (Id, Name, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
    (@TenantId1, N'Eco Tech',   1, GETUTCDATE(), NULL, NULL, NULL),
    (@TenantId2, N'Happy Tech', 1, GETUTCDATE(), NULL, NULL, NULL)

-- Users
DECLARE @UserId1 UNIQUEIDENTIFIER = 'B1000000-0000-0000-0000-000000000001'  -- admin  / Eco Tech
DECLARE @UserId2 UNIQUEIDENTIFIER = 'B2000000-0000-0000-0000-000000000002'  -- admin  / Happy Tech
DECLARE @UserId3 UNIQUEIDENTIFIER = 'B3000000-0000-0000-0000-000000000003'  -- user01 / Eco Tech

INSERT INTO Users (Id, UserName, PasswordHash, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
    (@UserId1, 'admin',  '$2a$11$pCWP4KV4y6FIIgmgz1clSOnZc2U0hck04pLY1fTXQixfcjo4I6yO2', 1, GETUTCDATE(), NULL, NULL, NULL),
    (@UserId2, 'admin',  '$2a$11$pCWP4KV4y6FIIgmgz1clSOnZc2U0hck04pLY1fTXQixfcjo4I6yO2', 1, GETUTCDATE(), NULL, NULL, NULL),
    (@UserId3, 'user01', '$2a$11$UgUF3vmk.MBmo4cLuaXMK.1oGzGH/.IoirvqCQB0Vi9ELv8nNCmWi', 1, GETUTCDATE(), NULL, NULL, NULL)

-- UserTenants  (Role: 0 = Member, 1 = Admin)
INSERT INTO UserTenants (Id, UserId, TenantId, Role, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES
    (NEWID(), @UserId1, @TenantId1, 1, 1, GETUTCDATE(), NULL, NULL, NULL),  -- admin   @ Eco Tech   = Admin
    (NEWID(), @UserId2, @TenantId2, 1, 1, GETUTCDATE(), NULL, NULL, NULL),  -- admin   @ Happy Tech = Admin
    (NEWID(), @UserId3, @TenantId1, 0, 1, GETUTCDATE(), NULL, NULL, NULL)   -- user01  @ Eco Tech   = Member

-- OAuth Clients  (ClientSecret plain: HRM_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'hrm',
    '$2a$11$p1HHS0KbZPWkiA0XxD1QyuMn1dC6O/RZJQordPNQxyOc4NPNMBz0i',
    N'HRM App',
    '["http://localhost:4200/sso-callback","https://task.happyecotech.com/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)

-- OAuth Clients: Mini CRM  (ClientSecret plain: MINI_CRM_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'mini-crm',
    '$2b$11$82q.xOjMDsIkozFdVsL8U.0EMROmho..AgdvFVfCie3bU3puLSWPu',
    N'Mini CRM App',
    '["http://localhost:4200/sso-callback","https://minicrm.happyecotech.com/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)

-- OAuth Clients: Cloud  (ClientSecret plain: CLOUD_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'cloud',
    '$2b$11$ikhG1kYbNCgVdv8OLXrbLuKmvkT0yOf2TWZFFM14VJT20X2dsadoa',
    N'Cloud App',
    '["http://localhost:4202/sso-callback","https://cloud.happyecotech.com/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)

-- Verify
SELECT u.UserName, t.Name AS TenantName,
       CASE ut.Role WHEN 1 THEN 'Admin' ELSE 'Member' END AS Role
FROM UserTenants ut
JOIN Users u   ON u.Id = ut.UserId
JOIN Tenants t ON t.Id = ut.TenantId
ORDER BY t.Name, u.UserName
