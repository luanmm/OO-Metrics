@ECHO OFF
 
echo Uninstalling WindowsService...
echo ---------------------------------------------------
set CURDIR=%~dp0
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil" /u "%CURDIR%OOM.Miner.exe"
echo ---------------------------------------------------
echo Done.
pause