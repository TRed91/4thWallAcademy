# 4th Wall Academy Documentation

> Author: Roth Thomas

last updated: 27-01-2024
Author: Roth Thomas

## Overview
The 4th Wall Academy Project is a multi project solution to manage Students, Sections, Courses, Instructors of the 4th Wall Academy as well as view reports about enrollment, grades, powers and weaknesses and schedules.

## Context
This project should provide an accessible way to manage and track data about the 4th Wall Academy.
Additionally to the Interface the project should provide an API that allows clients to consume information about class schedules.

## Solution
The Solution consist of multiple projects:
- Core Layer
- Data Layer
- Application Layer
- Testing Layer
- Model View Controller
- Rest API
- Security
- Logging
  
## Core Layer
Contains interfaces, models and entities that are shared across the solution

## Data Layer
Connects to the Database for adding, retrieving, updating and deleting data.
I choose Dapper over ADO.Net or Entity Framework to query the DB as it provides a good middleground between granularity and simplification. I also need complex queries like reading enrollment reports which will be easier to do with raw SQL.

### Tech
- SqlServer Database
- Dapper

## Application Layer
Contains testable business logic between the Data Layer and MVC / API Layer.
The application class methods return “result” types: { Ok: bool, Data?: <T>, message: string} which are consumed by the API and MVC Layer.
That way data and errors are being passed to the upper layers where they are handled properly.

## Testing Layer
Contains unit tests for testing logic of the Application Layer

### Tech
- Nunit
- Microsodt Test Sdk
  
## Model View Controller
Here the interface of the web application will be created using MVC workflow and Razor syntax.
Users need to authenticate via Alias and Password to view any of the sites content.
Users can have one of the following roles assigned to them:
- Admin: Access to all sites content and functionalities,
- Admissions: Can add new Students. This also creates an account with the student role and a random password.
- Manager: Can add, remove or update courses, sections and instructors
- Student: Can view only their schedule and student details. Account has to be created by a user with Admissions or Admin Role.
For the styling I leverage bootstrap classes for responsive design.
The header and footer are static while the options bar and main section are dynamic.

Users with the **Student** Role only have access to their Profile and Schedule pages.
The student’s profile shows all information including powers, weaknesses and sections. 
A user with the student role can only view their own profile and are redirected to an ‘unauthenticated error’ page if trying to view a profile with a different id in the url. The same goes for the schedule page.

Users with the **Admissions** role have access to the Students pages
Here users can view registered students, add new students and view student details.
The student details page is similar to the profile page but includes links to edit various data for a specific student.
The page for enrolling new students and editing existing student’s personal information are visually identical with the exception that fields in the edit form are already filled out.
Enrolling new Students also creates an account with a random password.
The pages for editing powers and weaknesses for a student are also similar.

Users with the **Manage** Role have access to the Manage Pages where Instructors, Courses and Sections can be added, updated and removed.
By default the Courses Subsection is selected. Here you can add a course or view course details.
The Edit button on Course Details page leads to an edit form that is visually identical to the ‘Add Course’ form. The Delete button redirects to a confirmation page before deleting the course.
**Caution**: Deleting a course also deletes all related sections!
Here sections can be created, updated, deleted and students can be enrolled or removed from sections.
The ‘Edit Section’ form is visually identical to the ‘Add Section’ form. The Sections End Time defaults to 45 minutes after the start time.
When hitting the Enroll Student link the user is redirected to a list of students to select from. A different version of the section details page is also accessible to students. The student version hides buttons and the list of enrolled students
Hitting the Enroll link adds the student to the section and redirects to the section details page, or shows an error if a student already is assigned to a course in that time frame.
Lastly the ‘Instructors’ subsection displays a list of instructors. Also new instructors can be added.
Again. The form for adding and editing an Instructor are visually identical.
The Terminate button redirects to a confirmation page before terminating an instructor.

Lastly users with the **Admin** Role have access to all sites content as well as the Admin Dashboard

The Enrollment Reports are divided in two subsections. ‘Sections’ and ‘Students’

### App Feedback and Error handling
Error messages, as well as success or informational messages are displayed below the header.
Error messages are displayed with red background and white text while success or informational messages are displayed with green background and white text.
Error messages can be displayed for various reasons, like database connection errors, form invalidity, and database creating/updating/deleting errors.
Every success, information and error message is also being logged to the console.
Upon error the user is redirected to either the previous page, home page or form page where an error message, as described above will be displayed.

## Rest API
The rest API allows external clients to receive information about schedules

### Endpoints:
| Endpoint     |	Path                                     |	Authentication, Roles         |	Method | Status Code | Returns |
| ------------ | ----------------------------------------- | ------------------------------ | ------ | ----------- | ------- |
| Login	       | /api/login                                |                                | Post	 | 200         | Json    |
| Schedule     | /api/schedule?’from-date’&’to-date’	     | Yes, Student                   | Get    | 200         | Json    |
| Any Schedule | /api/schedule/{id}/?’from-date’&’to-date’ | Yes, Admission, Manager, Admin |	Get    | 200         | Json    |

The Api only exposes two endpoints.
The **Login endpoint** requires users to send credentials in the request body
{
“Username”: “SolarFlare”
“Password”: “RandomPassword”
}
The “Username” field is the **Student Alias without any whitespace** (‘Solar Flare’ => ‘SolarFlare’) for **Students**. 
**Admins, Manager and Admission Users login with their Username**. 
A successful login attempt returns a json file in the response body containing a bearer token.

The **Schedule endpoint** requires authentication and returns a json file containing the schedule of the authenticated user only. If dates aren’t provided the schedule for a week starting from today is provided.
This endpoint also requires to specify the start and end date in the url query string following the format **fromDate=MM/dd/yyyy**

The **Any Schedule endpoint** allows to receive the schedule of any student by passing the student id.

### CORS policies
If the environment is set to development the **AllowAllOrigins** policy is used. Which allows all origins as the name suggests.
In production, the app uses the **AllowedOrigins** policy which reads the allowed origins from the appsettings.json file.

### Error Handling
Errors are being logged and controllers send an appropriate response of either 401 Unauthorized, 404 Not Found, 500 Internal Server Error etc.

## Security
The MVC and API will leverage **ASP Identity Entity Framework** to manage Accounts and Roles.
The Password requirements are:
- 8 characters minimal length
- at least one upper case character
- at least one lower case character
- at least one digit or other symbol
  
For the MVC there are the following roles:
- Student
- Admissions
- Manager
- Admin
  
When first registering to the MVC the user has no roles or privileges to view any of the sites content. The roles of Admissions, Manager or Admin have to be assigned by an Admin using good old SQL
The Student Roll is assigned when a new student is being added by an Admissions or Admin user.

## Logging
The MVC and API, by default, log information level events and above to the console.
I use **Serilog** for logging with Console and MSSqlServer Sink
- **Information Logs** on Database changes like creating, updating or deleting data.
- **Warning Logs** for invalid or unauthorized http requests.
- **Error Logs** for caught exceptions.
 
The applications write logs to the console and optionally to the database.

Database Logging is configurable in the appsettings.json of each program within the “DbLogging” section:
If “Enabled” is set to “true” the application writes events of the configured log level and above to the database.
The “LogLevel” can be set to:
- Information
- Debug
- Warning
- Error
  
The MVC and API write Logs to seperate Database Tables. “MVC_LogEvents”,  “API_LogEvents”
