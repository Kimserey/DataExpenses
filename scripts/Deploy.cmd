@echo off
cls
cd C:\Projects\DataExpenses

net stop london-expenses

REM ---------------------------------------
REM -     Build release
REM ---------------------------------------

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /p:configuration=release

pushd London.Web\bin\Release\

REM ---------------------------------------
REM -     Package in Release\Packages
REM ---------------------------------------

set package_name=data_expenses_package.zip
set packages_dir=Packages

if NOT EXIST %packages_dir% (
    mkdir %packages_dir%
)

REM a: archive command | -t{type of archive} | -x{exclude file | ! wildcard}
..\..\..\libs\7za.exe a -tzip -x!Packages %package_name% * 

move /y %package_name% %packages_dir%

REM ---------------------------------------
REM -     End
REM ---------------------------------------

popd London.Web\bin\Release\

net start london-expenses
