@echo off
cls
cd C:\Projects\DataExpenses

if EXIST London.Web\bin\Release\London.Web.exe (
	London.Web\bin\Release\London.Web.exe stop
)

REM ---------------------------------------
REM -     Build release
REM ---------------------------------------

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /p:configuration=release

pushd London.Web\bin\Release\

London.Web.exe start

popd London.Web\bin\Release\