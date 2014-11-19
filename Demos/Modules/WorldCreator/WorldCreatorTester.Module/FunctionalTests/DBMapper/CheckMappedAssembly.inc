*ReopenApplication
*Action Log On
*Action Navigation(WCDBMapper.TestTable)
*Action New(Test Table)
*FillForm
 Name = Tolis
*Action Save and Close
*Action Navigation(WCDBMapper.TestTable)
*CheckTable
 Columns = Name
 Row[0] = Tolis*

*Sql Integrated Security=SSPI;Data Source=.\SQLEXPRESS
 ScalarCommand = SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'TestTable'
 Result = 0