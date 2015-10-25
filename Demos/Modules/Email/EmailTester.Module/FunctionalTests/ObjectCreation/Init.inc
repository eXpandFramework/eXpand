#DropDB EmailTesterEasyTest
#Application EmailTester.Win
;#Application EmailTester.Web
*ScreenCapture
*UseModel ..\CommonSmtpClient;..\CommonReceipient;NewObject
*LogOn
*XpandDeleteFile Emails\*.eml
 InBin = true