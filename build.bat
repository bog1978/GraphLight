rem set MSBuild="%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\Tools\VsDevCmd.bat" 
MSBuild.exe "GraphLight.sln" /t:Rebuild