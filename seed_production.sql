-- ============================================================
-- SSO Production Seed Data
-- Chạy trên SsoDB production (Server 42.118.102.113,1433)
-- ============================================================

-- OAuth Clients
-- HRM (ClientSecret plain: HRM_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'hrm',
    '$2b$11$IPEaEJf/L6o/ldREeHlPweJT4Ol1BmktAZU0LiFeaS.duXlaUx.Ii',
    N'HRM App',
    '["https://task.happyecotech.com/sso-callback","http://localhost:4200/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)


-- Mini CRM (ClientSecret plain: MINI_CRM_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'mini-crm',
    '$2b$11$IPEaEJf/L6o/ldREeHlPweJT4Ol1BmktAZU0LiFeaS.duXlaUx.Ii',
    N'Mini CRM App',
    '["https://crm.happyecotech.com/sso-callback","http://localhost:4200/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)

-- Cloud (ClientSecret plain: CLOUD_SECRET)
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'cloud',
    '$2b$11$IPEaEJf/L6o/ldREeHlPweJT4Ol1BmktAZU0LiFeaS.duXlaUx.Ii',
    N'Cloud App',
    '["https://cloud.happyecotech.com/sso-callback","http://localhost:4202/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
)

-- Verify
SELECT ClientId, Name, AllowedRedirectUris FROM OAuthClients
