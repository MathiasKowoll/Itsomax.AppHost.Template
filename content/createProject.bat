:: Hide Commands
@echo off
set filename=%1

IF %1.==. set filename=NewSolution 

set modDir=Modules
set appDir=App
set solDir=Solution

set CurrentDirectory=%CD%
echo Current Directory = %CD%
echo "installing core modules"
pushd %CurrentDirectory%\%modDir%
echo Current Directory = %CD%

call git clone https://github.com/MathiasKowoll/Itsomax.Module.Core.git
call git clone https://github.com/MathiasKowoll/Itsomax.Module.UserCore.git
call git clone https://github.com/MathiasKowoll/Itsomax.Module.UserManagement.git
call git clone https://github.com/MathiasKowoll/Itsomax.Module.ItsomaxAdmin.git

echo "Install node modules"
pushd %CurrentDirectory%\%appDir%
call npm install --global gulp-cli
call npm install gulp
call npm install gulp-clean
call npm install glob

::echo "creating solution"
::echo Current Directory = %CD%
::dir /B *.csproj > dir_file.txt
::set /p file=< dir_file.txt  
::del dir_file.txt
set file=Itsomax.AppHost.csproj

echo "Adding Projects"
pushd %CurrentDirectory%\%solDir%
::set filename=%file:.csproj=%
call dotnet new sln -n %filename%
call dotnet sln add %CurrentDirectory%\%appDir%\%file%
call dotnet sln add %CurrentDirectory%\%modDir%\Itsomax.Module.Core\Itsomax.Module.Core.csproj
call dotnet sln add %CurrentDirectory%\%modDir%\Itsomax.Module.UserCore\Itsomax.Module.UserCore.csproj
call dotnet sln add %CurrentDirectory%\%modDir%\Itsomax.Module.ItsomaxAdmin\Itsomax.Module.ItsomaxAdmin.csproj
call dotnet sln add %CurrentDirectory%\%modDir%\Itsomax.Module.UserManagement\Itsomax.Module.UserManagement.csproj

echo "restoring packages"
call dotnet restore

echo "Build"
call dotnet build
echo "Copying modules"
pushd %CurrentDirectory%\%appDir%
call gulp copy-modules
call dotnet ef migrations add Initial_%filename%
