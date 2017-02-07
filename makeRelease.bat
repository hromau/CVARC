rd binaries /Q /S
md binaries 
cd uCvarc
"C:\Program Files\Unity\Editor\Unity.exe" -batchmode -executeMethod BuildScript.PerformBuild -quit
xcopy dlc ..\binaries\Dlc\ /E
cd ..
xcopy Commons\SingleplayerProxy\bin\Debug binaries\ /E
"C:\Program Files\7-Zip\7z.exe" a -tzip ucvarc.zip .\Binaries\





















