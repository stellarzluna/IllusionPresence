@echo off

:: WARNING
:: Don't run directly from explorer

:: this script needs https://www.nuget.org/packages/ilmerge

:: set your NuGet ILMerge Version, this is the number from the package manager install, for example:
:: PM> Install-Package ilmerge -Version 3.0.29
:: to confirm it is installed for a given project, see the packages.config file
SET ILMERGE_VERSION=3.0.29

:: the full ILMerge should be found here:
SET ILMERGE_PATH=.\packages\ILMerge.3.0.41\tools\net452

:: compiled dll path
SET COMPILED_PATH=.\IllusionPresence\bin\Debug

:: cd to this path
cd %*
echo Current Directory: %*

echo Merging Assembly...
IF NOT EXIST %ILMERGE_PATH%\ILMerge.exe echo ILMerge not found!

:: add project DLL's starting with replacing the FirstLib with this project's DLL
%ILMERGE_PATH%\ILMerge.exe /out:%COMPILED_PATH%\IllusionPresenceRelease.dll ^
  %COMPILED_PATH%\IllusionPresence.dll ^
  %COMPILED_PATH%\DiscordGameSDKCompiled.dll

:Done
echo Successfully merged in %COMPILED_PATH%
