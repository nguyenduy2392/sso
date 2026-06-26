-- Đăng ký Cloud làm OAuth Client trên SSO DB
-- ClientSecret plain: CLOUD_SECRET
INSERT INTO OAuthClients (Id, ClientId, ClientSecretHash, Name, AllowedRedirectUris, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (
    NEWID(), 'cloud',
    '$2b$11$ikhG1kYbNCgVdv8OLXrbLuKmvkT0yOf2TWZFFM14VJT20X2dsadoa',
    N'Cloud App',
    '["http://localhost:4202/sso-callback","https://cloud.happyecotech.com/sso-callback"]',
    1, GETUTCDATE(), NULL, NULL, NULL
);

SELECT ClientId, Name, AllowedRedirectUris FROM OAuthClients;
