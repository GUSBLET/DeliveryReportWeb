# DeliveryReportWeb
## Description:
It’s a convenient delivery report system that gives you an opportunity to send fast, view and manage the reports. There are 5 roles of users in the project such as: guest, user, deliveryman, moderator, administrator.  
- Guest can create a new account with mandatory link email confirmation and sing in his/her account.
- User can see profile information and edit account password and other data.
- Deliveryman is a role that can be assigned by the administrator or moderator. This role has the same ability as user also has additional features such as:
	- Create new report of work.
	- See his/her report history. 
-	Moderator is a role that can be assigned by the administration. This role has the same ability as user also has additional features such as:
	- View a list of users and information about users.
	- Manage user roles except for administrator and moderator roles.
	- View a list of delivery reports and information about reports.
	- You can create report excel file with reports that you can select with a filter.
- Administrator is a role that can be assigned once at the first start of the program. You can install number of users with this role in file “ApplicationDbContext.cs” in table “TableAccountUsers”. The number of these users may be unlimited. Administrator role has same ability as user and moderator, however administrator can manage users with the role of moderator.

## Features:
- You can select a language on the website between English and German.
- View information of report and user table by popup windows.
- Create a new account.
- Confirm your email account with the link on your E-mail.
- Change password.
- Use the report table and user management table with convenient navigation page. 
- Use filter to select information into the table.
- Create report excel file conveniently. 
- Change roles and users.

## Stack:
Backend:
-	ASP.NET 
-	Entity Framework
-	EPPlus
-	MailKit

Frontend:
-	HTML
-	CSS
-	Bootstrap

## How to install on your PC:
1.	Download Visual studio
2.	Install in “Visual Studio Installer” “ASP.NET and web-apps develops” package and Net7.0.
3.	Download and install git.
4.	Open git bash.
5.	Choose directory where you want to clone repository with the command: 
```bash
cd “Path to directory” 
Example: cd E:\Path
```
6.	Enter this command for clone repository: 
```bash 
git clone https://github.com/GUSBLET/DeliveryReportWeb.git
```
7.	Open solution file “DeliveryReportWeb.sln” with the Visual Studio.
8.	Add the next line to “appsettings.json”:
```c#
"ConnectionStrings": {DefaultConnection":"Server=(localdb)\\mssqllocaldb;Database=usersdb;Trusted_Connection=True;"  }
```
9.	 Open file the “MailService” and change the next line on your email address that will manage mail system.
```c# 
private readonly string _from = "Your email"
``` 
10.	 Open the next link and create special password for email: https://support.google.com/accounts/answer/185833?hl=en
11.	 Add to project new user secrets file and type there the next text:
```c#
"TokenMailService": "Your special password"
```
12.	Start the program.

