#Application ModelDifferenceTester.Web
#Application ModelDifferenceTester.Win
#IfDef ModelDifferenceTester.Web
*SqlDropDB 
 DBName = ModelDifferenceTesterEasyTest
#EndIf

*Action Log On
*ScreenCapture

#IncludeFile .\Win.inc
