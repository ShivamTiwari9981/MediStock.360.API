@echo off

cd /d C:\MediStock360\Api

set ASPNETCORE_URLS=http://localhost:5110

dotnet MediStock360.API.dll

pause