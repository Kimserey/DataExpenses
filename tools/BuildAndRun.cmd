@echo off
cls
cd C:\Projects\DataExpenses\

REM ---------------------------------------
REM -     Build debug
REM ---------------------------------------

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /p:configuration=debug


REM ---------------------------------------
REM -     Run debug
REM ---------------------------------------

pushd London.Web\bin\Debug

London.Web.exe -args:servicename=london-expenses-debug,url=http://localhost:9601

popd London.Web\bin\Debug