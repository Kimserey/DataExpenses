@echo off
cls

REM ---------------------------------------
REM -     Build release
REM ---------------------------------------

cd C:\Projects\DataExpenses
"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /p:configuration=release

pushd London.Web\bin

set release_dir=..\Release\
set packages_dir=..\Release\Packages\

if NOT EXIST %release_dir% (
    mkdir %release_dir%
)
if NOT EXIST %packages_dir% (
    mkdir %packages_dir%
)


REM ---------------------------------------
REM -     Deploy
REM ---------------------------------------

net stop london-expenses

REM /E for Copy of subfolders
REM /Y for Suppress prompt to confirm overwrite
xcopy . %release_dir% /ey

net start london-expenses


REM ---------------------------------------
REM -     Package in Release\Packages
REM ---------------------------------------

set package_name=data_expenses_package.zip

REM a: archive command
REM -t{type of archive}
..\..\libs\7za.exe a -tzip %package_name% * 


move /y %package_name% %packages_dir%


REM ---------------------------------------
REM -     End
REM ---------------------------------------

popd London.Web\bin