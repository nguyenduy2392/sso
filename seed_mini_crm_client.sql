-- Đăng ký mini-crm làm OAuth Client trên SSO DB
-- ClientSecret plain: MINI_CRM_SECRET
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'mini-crm',
    '$2b$11$82q.xOjMDsIkozFdVsL8U.0EMROmho..AgdvFVfCie3bU3puLSWPu',
    N'Mini CRM App',
    '["http://localhost:4200/sso-callback","https://minicrm.happyecotech.com/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
);

-- Verify
SELECT ClientId, Name, AllowedRedirectUris FROM OAuthClients;
