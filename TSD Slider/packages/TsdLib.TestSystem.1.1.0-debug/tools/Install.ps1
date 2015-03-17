 If(-Not (Test-Path C:\ProgramData\chocolatey))
 {
	 Install-Package chocolatey -Source nuget.org
	 Initialize-Chocolatey
	 Uninstall-Package chocolatey
	 choco install NuGet.CommandLine
	 #TODO: remove .nuget solution folder
 }