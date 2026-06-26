-- Fix redirect_uri: remove '#' from hash-routed URL (browser strips fragment before sending to server)
UPDATE OAuthClients
SET AllowedRedirectUris = '["http://localhost:4200/sso-callback","https://task.happyecotech.com/sso-callback","https://dev-task.happyecotech.com/sso-callback"]'
WHERE ClientId = 'hrm';

-- Verify
SELECT ClientId, AllowedRedirectUris FROM OAuthClients WHERE ClientId = 'hrm';
