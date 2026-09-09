-- Cập nhật AllowedRedirectUris cho client 'mini-crm' đã seed sẵn (chạy 1 lần trên SsoDB đã có dữ liệu)
-- Thêm http://localhost:4205/sso-callback (port dev thực tế của mini-crm/web, tránh trùng port 4200 của HRM)
-- và https://crm.happyecotech.com/sso-callback (domain production thực tế, theo nginx.conf)
UPDATE OAuthClients
SET AllowedRedirectUris = '["http://localhost:4200/sso-callback","http://localhost:4205/sso-callback","https://crm.happyecotech.com/sso-callback","https://minicrm.happyecotech.com/sso-callback"]'
WHERE ClientId = 'mini-crm';

-- Verify
SELECT ClientId, Name, AllowedRedirectUris FROM OAuthClients WHERE ClientId = 'mini-crm';
