@echo off

cls
color

SET solutiondir=C:\Projects\DataExpense

echo %solutiondir%

"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal /nologo /property:SolutionDir=%solutiondir%

pushd London.Web\bin

REM I use //+:9600 because I reserved the url

REM Topshelf command:
REM Install service with: London.Web.exe install -args=..,C:/Documents/Expenses,http://+:9600
REM Uninstall service with: London.Web.exe uninstall

REM Start, Stop service:
REM net start london-expenses
REM net stop london-expenses

London.Web.exe -args=..,C:/Documents/Expenses,http://+:9600

popd
popd