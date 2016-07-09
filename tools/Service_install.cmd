@echo off
cls

REM ---------------------------------------
REM -     Install service from Release
REM ---------------------------------------

echo %1 

C:\Projects\DataExpenses\London.Web\bin\Release\London.Web.exe install -args=%1