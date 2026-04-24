SELECT u."Email", u."IsEmailVerified", r."Name" as role 
FROM auth."Users" u 
LEFT JOIN auth."UserRoles" ur ON u."Id" = ur."UserId" 
LEFT JOIN auth."Roles" r ON ur."RoleId" = r."Id" 
WHERE u."Email" = 'deep.narayan@sierradev.com';
