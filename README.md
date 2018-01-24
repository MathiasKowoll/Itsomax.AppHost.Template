# IT-Somax AppHost Template

AppHost template is an easy way to create manage and develop modules and aplications using .Net Core. This project is base on modules you can develop using this template. This project is based on thiennn's work (https://github.com/simplcommerce/SimplCommerce).

## History and Comments

Based on my needs, I needed a way to create my own modularize project So I can build modules based on my needs and projects. The project consist on AppHost as the web container, Itsomax.Module.Core as Core package Itsomax.Module.UserCore as user management and role assigment, Itsomax.Module.ItsomaxAdmin as the main or principal web deployer and finally a library used to organized all the project. Right now it suppports Postgres and SQL Server. All bases configuration is in AppSettings.

I tried to use the more configuration fiendly without need of recompiling the whole apllication. you can change port database server user and port on the fly.

The application itself will create the database once the initial catalog has been created.

# Installation

1. install template: dotnet new -i Itsomax.AppHost.1.0.0
2. Using cmd go to root folder and execute createProject.bat "name of the project". This will create and restore all dependencies. Ex: createProject myNewProject.
3. Execute: dotnet run
4. Navigate to http://localhost:63097/
5. Default user is admin and Default Password is Admin123.,