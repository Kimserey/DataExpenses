@echo off

cls
color

SET solutiondir=C:\Projects\DataExpense

echo %solutiondir%

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /nologo /property:SolutionDir=%solutiondir%

pushd London.Web\bin

REM I use //+:9600 because I reserved the url
London.Web.exe ".." "http://+:9600"

