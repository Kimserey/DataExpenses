@echo off

set package_name=package.zip
set release_dir=..\Release\Packages

pushd London.Web\bin

REM a: archive command
REM -t{type of archive}
REM -x{exclude file/folder - ! wildcard}
..\..\libs\7za.exe a -tzip -x!Debug %package_name% * 

if NOT EXIST %release_dir% (
    mkdir %release_dir%
)

move /y %package_name% %release_dir%

popd London.Web\bin