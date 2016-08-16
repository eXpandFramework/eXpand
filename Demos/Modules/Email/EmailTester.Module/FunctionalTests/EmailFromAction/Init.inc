#DropDB EmailTesterEasyTest
#Application EmailTester.Win
#Application EmailTester.Web
*ScreenCapture
*UseModel ..\CommonSmtpClient;..\CommonReceipient;EmailFromAction
*LogOn
*XpandDeleteFile %temp%\Emails\*.eml
 InBin = true
