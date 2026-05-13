@echo off
chcp 65001 >nul
title Fix Water3 API Port 8443

echo =====================================
echo Fix Water3 API Port 8443
echo =====================================
echo.

echo Closing possible old registrations...
netsh http delete urlacl url=http://+:8443/
netsh http delete urlacl url=http://*:8443/
netsh http delete urlacl url=https://+:8443/
netsh http delete urlacl url=https://*:8443/

echo.
echo Adding URL ACL permission...
netsh http add urlacl url=https://+:8443/ sddl=D:(A;;GX;;;WD)

echo.
echo Updating firewall rule...
netsh advfirewall firewall delete rule name="Water3 API 8443"
netsh advfirewall firewall add rule name="Water3 API 8443" dir=in action=allow protocol=TCP localport=8443

echo.
echo Checking port usage...
netstat -ano | findstr :8443

echo.
echo Done.
echo Restart Windows, then run Water3 normally.
pause