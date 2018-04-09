#DropDB ExcelImporterTesterEasyTest
#DropDB ExcelImporterTesterEasyTest
#Application ExcelImporterTester.Win
#Application ExcelImporterTester.Web
*ScreenCapture
*LogOn

*Action Navigation(Default.Excel Import)
*Action New
;---------------------Save Validation Rules----------------
*Action Save
*XpandCheckValidationResult
 Info  = "Name" must not be empty.

*ExecuteEditorAction File
*OpenFileDialog [MasterData]

;---------------------Map Validation Rules----------------
*Action Map
*XpandCheckValidationResult
 Info  = "Sheet Name" must not be empty.
 Info  = "Type" must not be empty.

*XpandFillForm
 Sheet Name = Active_List
 Type = Customer
 Use Header Rows = True
;--------------------Map------------------------
*Action Map
*CheckTable Excel Column Maps
 Columns = Excel Column Name, Property Name
 Row[0] = Gender, Gender
 Row[1] = ImportName, ImportName

;----------------------Import-------------
*Action Import
#IfDef ExcelImporterTester.Win
*HandleDialog
 Message = Importing succeeded for all 48 rows. Press Save to commit.
 Respond = OK
#EndIf
#IfDef ExcelImporterTester.Web
*HandleDialog
 Message = Importing succeeded for all 48 rows. Press Save to commit.
#EndIf
*XpandFillForm
 Name = Test
*SaveAndClose
*Action Navigation(Default.Customer)
#IfDef ExcelImporterTester.Win
*CheckTable 
 RowCount = 48
 Columns = ImportName, Gender
 Row[0] = 'Aayush  ', 'Male'
#EndIf
#IfDef ExcelImporterTester.Web
*CheckTable 
 RowCount = 48
 Columns = ImportName, Gender
 Row[0] = 'Aayush', 'Male'
#EndIf
*Action Navigation(Default.Excel Import)

;--------------import with stored configuration-------

*ProcessRecord
 Name = Test
*ExecuteEditorAction File
*OpenFileDialog [MasterData]
*Action Import

#IfDef ExcelImporterTester.Win
*HandleDialog
 Message = Importing succeeded for all 48 rows. Press Save to commit.
 Respond = OK
#EndIf
#IfDef ExcelImporterTester.Web
*HandleDialog
 Message = Importing succeeded for all 48 rows. Press Save to commit.
#EndIf

;------------------------invalidate existing mapping-------------
