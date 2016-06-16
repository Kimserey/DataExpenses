@echo off

cls
color

SET solutiondir=C:\Projects\DataExpense

echo %solutiondir%

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /nologo /property:SolutionDir=%solutiondir%

pushd ".\bin"
London.Web.exe ".." "http://+:9100"
popd

