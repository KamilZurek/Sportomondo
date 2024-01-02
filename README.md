# Sportomondo

### About this project: ###

Thinking about "what is the best way to learn WebAPIs" I've decided to create this project.
The idea of "activity tracker" came up in my head with a simple reason - I really like running and riding bicycle (maybe triathlon in the future) and
I'm a daily user of Garmin Connect app, prior - Endomondo (a little bit inspiration with its name :D).
So what was the better solution to implement than something I use quite a lot and it's not Facebook? :)

Generally it's a REST WebAPI which tracks Your activity (workouts) by saving data in a database and has some extra features like summaries or achievement system. In addition, it lets You manage its users, roles and their permissions.

More details below :)

### Tech stack: ###

- C#
- ASP .NET Core 6
- MS SQL Server (2019)
- Entity Framework Core
- xUnit

### Description: ###

- relational MS SQL Server database (diagram at the bottom) with relationships: One-to-one, One-to-many, Many-to-many, using Entity Framework Core (Code-First approach with migrations)
- JWT Bearer authentication & authorization:
	- user can authenticate by signing in using its email address and password. If valid - in response user gets a JWT Token with expiration date.
	- each endpoint has its own authorization policies, which are stored in database as "RolePermisssions" for each user's role. When user calls specific endpoint, API checks user's claims in JWT Token (especially "Role" claim), matches it with endpoint's policy name, searches and checks in database if RolePermission flag "Enabled" is true - then user is authorized.
- asynchronous endpoints and database / web calls with CancellationToken
- dependency injection approach
- request models validation (using FluentValidation and attributes)
- mapping responses to DTO objects
- middleware to handle different types of exceptions and return HTTP StatusCodes
- logging errors to file (using NLog)
- CORS (basic configuration only for presentation purposes - allow any origin)
- hashing users' passwords in database
- controllers & models:
	- achievement:
		- "Achievement" is an object which represents challenge that users can complete (for example - "Run 100km - Reward: 5 points")
		- endpoints: GetAll, Create, Delete, Check (check and assign achievements to users)
	- activity:
		- "Activity" is an object which represents user's workout with details and it has connection to the "Weather" object
		- endpoints: GetAll, Get, Create, Delete, Update
		- 3 predefined activity types: Running, Cycling, Swimming
	- summary:
		- "Summary" is an object which represents dynamically computed a total of data registered by current user (for example - TotalDistance for each activity type, Achievements points) 
		- endpoints: Get
	- user:
		- "User" is an object which represents a person who uses this API with specific role
		- endpoints: Register, Login, ChangePassword, GetAll, Delete, ChangeRole
		- 3 predefined roles for users: Admin (high permissions), Support (medium permissions), Member (lowest permissions):
			- all role's permissions can be changed individually in database / json file (when seeding database)
			- "RolePermission" is an object which tells if user's role is authorized for particular action (flag "Enabled")
- using external APIs:
	- to get Weather object for Activity (WeatherAPI)
	- to get activity chart for Summary (QuickChartAPI)
- seeding database feature:
	- applying pending migrations
	- seeding data with roles, role permissions and user (Admin)
- xUnit integration tests for ActivityController:
	- database: EntityFrameworkCore.InMemory approach
	- fake authentication & authorization
	- shared context between tests using IClassFixture and WebApplicationFactory
	- FluentAssertions
- Swagger as documentation & testing tool (or Postman)

### Further development: ###

- API versioning

### Database diagram: ###

![image](https://github.com/KamilZurek/Sportomondo/assets/107115837/47c4ed2e-7c7b-4eb2-9891-d5be3df392ca)

### Swagger preview: ###

![image](https://github.com/KamilZurek/Sportomondo/assets/107115837/4e2bde9f-61d0-47f9-8287-a37f630be276)

