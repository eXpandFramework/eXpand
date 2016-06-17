#DropDB WorldCreatorTesterEasyTest
#DropDB WorldCreatorTesterCreator
#DropDB WCDBMapperTester
#Application WorldCreatorTester.Win
#Application WorldCreatorTester.Web
*LogOn
*ScreenCapture

;-----------create dummy db to be mapped------------
*Sql 'Integrated Security=SSPI;Data Source=(localdb)\MSSQLLocalDB'
 Command1 = CREATE DATABASE WCDBMapperTester
 Command2 = USE WCDBMapperTester CREATE TABLE WCTestTable(Oid [int] IDENTITY(1,1) NOT NULL,Name [nchar](10) NULL, CONSTRAINT PK_WCTestTable PRIMARY KEY CLUSTERED (Oid ASC) ON [PRIMARY]) ON [PRIMARY]