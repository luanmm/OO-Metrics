@ECHO OFF
 
echo Installing WindowsService...
echo ---------------------------------------------------
set CURDIR=%~dp0
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil" /i "%CURDIR%OOM.Miner.exe"
echo ---------------------------------------------------
echo Done.
pause