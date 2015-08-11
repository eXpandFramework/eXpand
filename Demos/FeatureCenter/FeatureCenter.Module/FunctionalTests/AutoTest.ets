#Timeout 160
#DropDB XpandFeatureCenterEasyTest
#DropDB NorthWind
#DropDB XpandFeatureCenterExceptionHandling
#DropDB XpandFeatureCenterMultipleDataStore
#DropDB XpandFeatureCenterWorldCreator
#Application FeatureCenter.Win
#Application FeatureCenter.Web

*SetWebMaxWaitTimeOut 50000
 
*ScreenCapture
*FillForm
 User Name = Admin
 Password = Admin
*Action Log On

*XpandAutoTest

