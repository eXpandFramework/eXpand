#DropDB WorldCreatorTesterEasyTest
#DropDB WorldCreatorTesterCreator
#DropDB WCDBMapperTester
#Application WorldCreatorTester.Win
;#Application WorldCreatorTester.Web
*LogOn
*ScreenCapture

*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS
 Command1 = CREATE DATABASE WCDBMapperTester
 Command2 = USE WCDBMapperTester CREATE TABLE TestTable(Oid [int] IDENTITY(1,1) NOT NULL,Name [nchar](10) NULL, CONSTRAINT PK_TestTable PRIMARY KEY CLUSTERED (Oid ASC) ON [PRIMARY]) ON [PRIMARY]