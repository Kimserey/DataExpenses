@echo off
cls
cd ..

REM ---------------------------------------
REM -     Build debug
REM ---------------------------------------

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /p:configuration=debug


REM ---------------------------------------
REM -     Run debug
REM ---------------------------------------

pushd London.Web\bin

London.Web.exe -args=..,C:/Documents/Expenses,http://+:9600

popd London.Web\bin