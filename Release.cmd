@echo off

cd C:\Projects\DataExpenses
"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe" /maxcpucount /verbosity:minimal

net stop london-expenses

REM /E for Copy of subfolders
REM /Y for Supress prompt to confirm overwrite

xcopy London.Web\bin London.Web\Release /ey

net start london-expenses