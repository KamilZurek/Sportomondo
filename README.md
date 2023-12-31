# Sportomondo

About this project / Intro?:

Thinking about "what is the best way to learn WebAPIs" I've decided to create this project.
The idea of "activity tracker" came up in my head with a simple reason - I really like running and riding bicycle (maybe triathlon in the future) and
I'm daily user of Garmin Connect app, prior - Endomondo (a little bit inspiration with its name :D).
So what was the better solution to implement than something I use quite a lot and it's not Facebook? :)

Generally it's a REST WebAPI which track Your activity (workouts) by saving data in database, which has some extra features like summaries and achievment system, in addition lets You manage its users, roles and thier permissions.
More details below :)

<wrocic tu po napisanie opisu>

Tech stack:

C#
ASP .Net Core 6
MS SQL Server (2019)
Entity Framework Core
xUnit
NLog
JWT Bearer
Swagger

Things I implemented:

- relational MS SQL Server database (diagram at the bottom) with relationships: One-to-one, One-to-many, Many-to-many, using Entity Framework Core (Code-First approach with migrations)
- JWT Bearer authentication & authorization:
	- user can authentice by signing in using its email address and password (which are hashed in database)
	- each endpoint has its own authorization policy, which are stored in database as "RolePermisssions" for each user's role. When user calls specific endpoint, API checks user's claims in JWT Token (especially "Role" claim), match it with endpoint's policy name, searchs and checks in database if RolePermission flag "Enabled" is true - then user is authorized.

Further development:

?How to try it:

//if azure

Database diagram:

![image](https://github.com/KamilZurek/Sportomondo/assets/107115837/47c4ed2e-7c7b-4eb2-9891-d5be3df392ca)

