@echo off
cd /d "%~dp0"
net session >nul 2>&1
if %errorlevel% == 0 (
reg add HKCR\.cvarcreplay /ve /t REG_SZ /d CvarcReplay /f
reg add HKCR\CvarcReplay /ve /t REG_SZ /d "Replay of CVARC game" /f
reg add HKCR\CvarcReplay\shell\Open\Command /ve /t REG_SZ /d "%cd%\Binaries\LogPlayer.exe "%%1"" /f
) else (
echo ERROR! Need admin permissions
pause
)