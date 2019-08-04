[![Financial Contributors on Open Collective](https://opencollective.com/eXpand/all/badge.svg?label=financial+contributors)](https://opencollective.com/eXpand) 

[![Custom badge](https://img.shields.io/endpoint.svg?label=Nuget.org&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpand)](https://www.nuget.org/packages?q=expandframework) [![](https://img.shields.io/github/downloads/eXpand/expand/total.svg?label=github%20downloads&style=flat)](releases.expandframework.com)

[![GitHub stars](https://img.shields.io/github/stars/eXpandFramework/eXpand.svg)](https://github.com/eXpandFramework/eXpand/stargazers) **Star the project if you think it deserves it.** 

[![GitHub forks](https://img.shields.io/github/forks/eXpandFramework/eXpand.svg)](https://github.com/eXpandFramework/eXPand/network) **Fork the project to extend and contribute.**

<img src="http://logo.expandframework.com" width=150 height=68 alt="eXpandFramework logo"/> | Nuget
|--------|--------
[![GitHub release](https://img.shields.io/github/release/expandframework/expand.svg)](https://github.com/expandframework/expand/releases/latest) ![GitHub Releases](https://img.shields.io/github/downloads/expandframework/expand/latest/total.svg?label=Github) ![Custom badge](https://img.shields.io/endpoint.svg?label=Nuget&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2Flatest%3Fid%3DeXpand)|```nuget.exe list author:eXpandFramework``` 
[![](https://img.shields.io/azure-devops/build/eXpandDevOps/dc0010e5-9ecf-45ac-b89d-2d51897f3855/32/master.svg?label=Lab%20build&style=flat)](https://dev.azure.com/eXpandDevOps/eXpandFramework/_build/latest?definitionId=32&branchName=master)|```nuget.exe list Xpand -source https://xpandnugetserver.azurewebsites.net/nuget```

<sub><sup>[How do I set up a package source in Visual Studio?](https://go.microsoft.com/fwlink/?linkid=698608), [Efficient package management](https://github.com/eXpandFramework/DevExpress.XAF/wiki/Efficient-package-management)<br>[How to Debug](https://github.com/eXpandFramework/eXpand/wiki/HowToDebug), [How to boost your DevExpress Debugging Experience](https://github.com/eXpandFramework/DevExpress.XAF/wiki/How-to-boost-your-DevExpress-Debugging-Experience)</sup></sub>


[<img src="https://img.shields.io/badge/Search-ReleaseHistory-green.svg"/>](https://github.com/eXpandFramework/eXpand/tree/master/ReleaseNotesHistory) [![Join the chat at https://gitter.im/xpandframework/community](https://badges.gitter.im/xpandframework/community.svg)](https://gitter.im/XpandFramework/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

[![GitHub open issues](https://img.shields.io/github/issues/eXpandFramework/eXpand.svg)](https://github.com/eXpandFramework/eXpand/issues) [![GitHub close issues](https://img.shields.io/github/issues-closed/eXpandFramework/eXpand.svg)](https://github.com/eXpandFramework/eXpand/issues?q=is%3Aissue+sort%3Aupdated-desc+is%3Aclosed)

# About
![open collective backers and sponsors](https://img.shields.io/opencollective/all/expand.svg?label=If%20this%20organization%20helped%20your%20business%2C%20we%20kindly%20request%20to%20consider%20sponsoring%20our%20activities)

_eXpandFramework_ is an independent open-source project and is not affiliated with Developer Express Inc.

_eXpandFramework_ is a **FREE** extension for **DevExpress-XAF** and operates within the **Microsoft Public License (Ms-PL)**. 

The [eXpandFrameowork](https://github.com/eXpandFramework) organization consist of two projects. The main project, this one, which unfortunately lacks detailed documentation, and the Standalone low dependency well tested and well documented [DevExpress.XAF](https://github.com/eXpandFramework/DevExpress.XAF) repository. The main project acts as a container to the standalone proejct. Eventually all modules of main project will be puzzled from the standalone modules. The framework is transitioning fast from private server to GitHub since Oct 2018, so please bear with us and contribute where you can.

This framework has similar architecture to XAF. We recommend you go through the [Getting Started](https://github.com/eXpandFramework/eXpand/wiki/Getting-Started) document to get the feeling on how to get the most out of it. Afterwards just installed the modules you want and go through the blogs linked in the list below and feel free to post [Issues](https://github.com/eXpandFramework/eXpand/issues).

## Modules

Examples of those modules include (in the two right columns you can see the supported platform):

Module Name | Description | ![Windows](http://www.expandframework.com/images/site/windows.jpg "Windows") | ![ASPNET](http://www.expandframework.com/images/site/aspnet.jpg "ASPNET") | Nuget.org
------------|-------------|---------|-------|---------
[ExcelImporter](#excelimporter) |    Imports Excel, csv files. |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandExcelImporter%2A)
[WorldCreator](#worldcreator) |  Design runtime assemblies | Y | Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandWorldCreator%2A)
[ModelDifference](#model-difference) | Model management | Y | Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandModelDifference%2A)
[Dashboard](#dashboard) |    Enables native XAF dashboard collaboration and integrates the Dashboard suite |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpand%2ADashboard%2A)
Email |     Send emails using business rules from application model without coding (see <http://goo.gl/Hkx6PK)> |    Y|Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandEmail)
[WorkFlow](#workflow) |    Contains workflow related features (Scheduled workflows) |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandWorkFlow%2A)
[System](#system-modules)|    Support multiple datastore , calculable properties at runtime ,dynamic model creation,control grid options, datacaching, web master detail, view inheritance etc.    |Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandSystem%2A)
JobSheduler    | Acts as a wrapper for the powerfull Quartz.Net, providing a flexible UI for managing Jobs at runtime |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandJob%2A)
[DBMapper](#dbmapper) |    Map 14 different types of databases at runtime into worldcreator persistent objects    | Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandWorldCreatorDbMapper)
[IO](#io) |    Export & Import object graphs |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandIO%2A)
MapView |     Google Maps integration for XAF web apps. Blog posts. |     Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandMapView%2A)
FileAttachments |     Provides support for file system storage as per E965 |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandFileAttach%2A)
Scheduler |     Please explore the XVideoRental module found in Demos/XVideoRental folder (Blog posts) |    Y |    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandScheduler%2A)
Reports |     Please explore the XVideoRental module found in Demos/XVideoRental folder    | Y |    N|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandReports%2A)
Chart |      Please explore the XVideoRental module found in Demos/XVideoRental folder     |Y |    N|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandChart%2A)
PivotGrid |      Please explore the XVideoRental module found in Demos/XVideoRental folder |    Y |    N| ![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandPivotGridWin)
[Import Wizard](#importwizard) |    Universal module for importing excel files into any XAF application.     |Y|    N|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandImportWizard%2A)
AuditTrail    |Configures XAF Audit Trail module using the Application Modules. (see Declarative data auditing)|    Y|     Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandAuditTrail%2A)
[StateMachine](#statemachine)|    Enhance XAF's statemachine module in order to control transitions using permissions    |Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandStateMachine%2A)
[Logic](#logic)|    Define conditional architecture    |Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandLogic%2A)
[ModelArtifact](#modelartifactstate)|    Parameterize model artifacts (Controllers, Actions, Views)|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandModelArtifact%2A)
[AdditionalViewControlsProvider](#additionalviewcontrolsprovider)    |Decorate your views with custom controls|    Y    |Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandAdditionalViewControlsProvider%2A)
MasterDetail|    XtraGrid support for master-detail data presentation using the model.    Y|    N||![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandMasterDetai%2A)
[PivotChart](#pivotchart)|    Enhance analysis procedures / controls|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandPivotChar%2A)
Security|    Provides extension methods, authentication providers, login remember me, custom security objects|     Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandSecuri%2A)
[Wizard](#wizardui)|    Design wizard driven views|    Y    |N|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandWizard%2A)
[ViewVariants](#viewvariants)|    Create views without the use of model editor|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandViewVariants%2A)
[Validation](#validation)|    More rules , permission validation, warning/info support, Action contexts etc|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandValidation%2A)
ConditionalObjectViews|    Allows the conditional navigation to your detailviews/listviews-->Merged with ModelArtifact    |Y|    Y
EasyTests|    Custom command and extensions for EasyTest see <http://apobekiaris.blogspot.gr/search/label/EasyTest>|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandEasyTes%2A)
[TreelistView](#treelistview)|    Enhance hierarchy controls, map XtraTreeList options to model|    Y|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandTreelistView%2A)
[NCarousel](#ncarousel)|    Loads images asynchronously and displays them using a configurable carousel listeditor|    N|    Y|![Custom badge](https://img.shields.io/endpoint.svg?color=%20&label=%20&url=https%3A%2F%2Fxpandnugetstats.azurewebsites.net%2Fapi%2Ftotals%2FeXpandNCarousel%2A)
VSIX Package|    Enhance Model Editor, Explore Xaf Errors, Drop Database at design time, ProjectConverter invocation|

### Dashboard

Blogs:\
<http://apobekiaris.blogspot.gr/search/label/dashboard>

### Model Difference

Extends XAF by adding great new features for example:

* the ability to generate runtime members for your objects
* creating Application/Role/User models in the database
* storing your web cookies in the database
* handling of external application models
* combine end user modifications with application model
* support for multiple models at design time

<a href="http://lh3.ggpht.com/_5YPm4JGkfwE/TKS-Dto4vJI/AAAAAAAAA2M/23ElRzSeGxY/s1600-h/1%5B4%5D.png" ><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TKS-EDVfWVI/AAAAAAAAA2U/C4q0TgSBQxA/1_thumb%5B2%5D.png?imgmax=800" width="244" height="119"/></a>|
<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKS-GpI3V0I/AAAAAAAAA2g/ir7JM5yPYAI/s1600-h/2%5B4%5D.png" target="_blank"><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TKS-He26FgI/AAAAAAAAA2k/sGqr-d1q8no/2_thumb%5B2%5D.png?imgmax=800" width="244" height="146"  /></a>

Blogs:\
<http://apobekiaris.blogspot.com/search/label/ModelDifference>

### WorldCreator
Creates dynamic persistent assemblies. The XAF user interface allows us to create an assembly without writing a single line of code. Advanced users can even use c# scripting and create new code generation templates.  

<a href="http://lh5.ggpht.com/_5YPm4JGkfwE/TKS-JsXS8zI/AAAAAAAAA2s/GealQVYGk2I/s1600-h/3%5B4%5D.png" target="_blank"><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TKS-KYRSs6I/AAAAAAAAA20/RBORqaxDbQk/3_thumb%5B2%5D.png?imgmax=800" width="244" height="132"/></a>

Blogs:\
<http://apobekiaris.blogspot.com/search/label/WorldCreator>

### JobScheduler
For those who are not familiar with the concept of job scheduling, [here](http://en.wikipedia.org/wiki/Job_scheduler) is some background information. Also, there is great Quartz [tutorial](http://quartznet.sourceforge.net/tutorial/index.html), which provides an excellent introduction.

Blogs:\
<http://apobekiaris.blogspot.com/search/label/JobSceduler>

### DBMapper
DBMapper has the same functionality as SqlDbMapper however is based on the powerful [XPO](http://www.devexpress.com/Products/NET/ORM/) . Thus  DBMapper module is capable of transforming into WorldCreator persistent classes the following list of databases: Access, [Advantage](http://www.sybase.com/products/databasemanagement/advantagedatabaseserver), [Adaptive Server Anywhere](http://www.isug.com/Sybase_FAQ/ASA/index.html), Ase, [DB2](http://www-01.ibm.com/software/data/db2/), [FireBird](http://www.firebirdsql.org/), [FireBirdSql](http://www.firebirdsql.org/), [SqlServerCe](http://www.microsoft.com/sqlserver/en/us/editions/compact.aspx), [SqlServer](http://www.microsoft.com/sqlserver/en/us/default.aspx), [ODP](http://www.oracle.com/technetwork/topics/dotnet/index-085163.html),  [Oracle](http://www.oracle.com/index.html), [PersasizeSql](http://www.pervasive.com/), [PostgreSql](http://www.postgresql.org/), [SQLite](http://www.sqlite.org/), [VistaDB](http://www.vistadb.net/)  

<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKS-NKe9gpI/AAAAAAAAA3E/sQhwSk0lqXA/s1600-h/4%5B4%5D.png" target="_blank" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKS-NnEnx1I/AAAAAAAAA3M/3RRz9c0dvYQ/4_thumb%5B2%5D.png?imgmax=800" alt="4" width="244" height="137"  /></a>

Blogs:\
<http://apobekiaris.blogspot.com/search/label/SqlDbMapper>

### Import Export Module
Provides collaboration between a xaf application and another system by allowing you to create object serialization graphs. The complexity of a graph can be configured using either UI or design time attributes. Changing object keys and choosing serialization strategies are straightforward as seen in this screenshot.

<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TKS-RG9vtqI/AAAAAAAAA3Q/atE-te0RBpc/s1600-h/5%5B4%5D.png" target="_blank" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKS-SBm18GI/AAAAAAAAA3k/MKVWkLOblqo/5_thumb%5B2%5D.png?imgmax=800" alt="5" width="244" height="175"/></a>|<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TKTGP8hA8jI/AAAAAAAAA8A/VPi85wauaew/s1600-h/6%5B11%5D.png" target="_blank" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGRLKZmZI/AAAAAAAAA8E/IoJPGHtg4nU/6_thumb%5B5%5D.png?imgmax=800" alt="6" width="244" height="203"  /></a>\
<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGRoqHwvI/AAAAAAAAA8I/NnOAO9BJ-SY/s1600-h/7%5B8%5D.png" target="_blank" ><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TKTGSVcaUII/AAAAAAAAA8M/cHWqle8GUUs/7_thumb%5B4%5D.png?imgmax=800" alt="7" width="244" height="175" /></a>\
Blogs:\
<http://apobekiaris.blogspot.com/search/label/IO>

### Import Wizard
Windows module for importing excel files into any XAF application.\
<img src="http://1.bp.blogspot.com/_qoNyZHelrQw/TGv4_qFc08I/AAAAAAAAQu4/baach20MnHk/s640/Excel+Import+3.png" alt="Import Wizzard Preview" width="244" />\
Blogs:\
http://mdworkstuff.blogspot.gr/search/label/wizard

### AdditionalViewControlsProvider
Allows us to place a Win or Web control in any given position in its respective XAF view and control its visibility, text decoration, font, height, width, etc. using conditional model rules.\
<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGS0pCiuI/AAAAAAAAA8Q/lMY4nHkkeJk/s1600-h/8%5B9%5D.png" target="_blank" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGTiQHUVI/AAAAAAAAA8U/VoWwfpX--_4/8_thumb%5B5%5D.png?imgmax=800" alt="8" width="244" height="188" /></a>|<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGUkErwRI/AAAAAAAAA8Y/Ymr3orssvz8/s1600-h/9%5B8%5D.png" target="_blank" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGVNRtVuI/AAAAAAAAA8c/LglYVygFdKg/9_thumb%5B4%5D.png?imgmax=800" alt="9" width="244" height="108" /></a>\
Blogs:\
http://apobekiaris.blogspot.com/search/label/AdditionalViewControlsProvider

### ModelArtifactState
Apply conditional logic to the artifacts of your model (Controllers, Actions). It means that you can enable,disable,hide,execute etc. at many contexts or combination of them.

<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TKTGWzkYSJI/AAAAAAAAA8g/cWtL1GB9XRA/s1600-h/10%5B8%5D.png" target="_blank" ><img src="http://lh3.ggpht.com/_5YPm4JGkfwE/TKTGYHIKvXI/AAAAAAAAA8k/O1JYHungAdE/10_thumb%5B4%5D.png?imgmax=800" alt="10" width="222" height="244"  /></a>\
Blogs:\
http://apobekiaris.blogspot.com/search/label/ModelArtifactState

### StateMachine
Enhance XAF's statemachine module in order to control transitions using permissions.

Blogs:\
http://apobekiaris.blogspot.com/search/label/Xpandstatemachine  

### PivotChart 
Using it you can connect your analysis objects with any object and display/position them at any view. A big number of pivot features are also supported  . Controlling Pivotgrid options is also available at runtime.

<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGa5lGuVI/AAAAAAAAA8w/Jp7oyPEq48c/s1600-h/12%5B9%5D.png" target="_blank" rel="noopener" ><img src="http://lh3.ggpht.com/_5YPm4JGkfwE/TKTGbx4fMxI/AAAAAAAAA80/fyszg5Wy_7U/12_thumb%5B5%5D.png?imgmax=800" alt="12" width="244" height="159" /></a>|<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TLYN-Fe5lrI/AAAAAAAAA-k/W9HIdgO-7ng/s1600-h/image%5B4%5D.png" target="_blank" rel="noopener" ><img src="http://lh3.ggpht.com/_5YPm4JGkfwE/TLYN_K3cjnI/AAAAAAAAA-o/6uR5EOP7gbE/image_thumb%5B1%5D.png?imgmax=800" alt="image" width="244" height="171" /></a>\
Blogs:\
http://apobekiaris.blogspot.com/search/label/PivotChart

### Logic
Provides the architecture / engine upon many of eXpand modules have been designed (AdditionalViewControlsProvider, ModeArtifactState, MasterDetail, ConditionalDetailViews) . Very valuable to the developers that want to implement a conditional module.

Blogs:\
http://apobekiaris.blogspot.com/search/label/Logic 

### System modules 
The features here are endless . Some interesting features are multiple datastore support, create calculable properties at runtime, dynamic model creation, GridView,GridColumn options…etc.

Blogs:\
http://apobekiaris.blogspot.com/search/label/Core.Web\
http://apobekiaris.blogspot.com/search/label/Core.Win\

### ConditionalObjectView

Allows the conditional navigation to your detailviews / listviews.

<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TKTGe7dYA0I/AAAAAAAAA9I/OuN5Cs7uVAw/s1600-h/15%5B9%5D.png" target="_blank" rel="noopener" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGfoUv_gI/AAAAAAAAA9M/Wlt00bo5UFM/15_thumb%5B5%5D.png?imgmax=800" alt="15" width="244" height="129"  /></a>


### ViewVariants 
End user can design views at runtime.

<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TKTGhYoMiKI/AAAAAAAAA9Q/MowHd45EMwA/s1600-h/16%5B9%5D.png" target="_blank" rel="noopener" ><img src="http://lh3.ggpht.com/_5YPm4JGkfwE/TKTGh6wP6yI/AAAAAAAAA9U/3XO4zHU3fy4/16_thumb%5B5%5D.png?imgmax=800" alt="16" width="244" height="126" /></a>

Blogs:\
http://apobekiaris.blogspot.com/search/label/ViewVariants

### Validation
![Validation](http://www.expandframework.com/media/kunena/attachments/65/detailviewwarning.png)

Blogs:\
http://apobekiaris.blogspot.com/search/label/Validation\
([warning/Info](http://www.expandframework.com/forum/8-discussion/1319-validation-rule-warning-support.html))

### WizardUI
Enables designing of wizard views using the model editor.

<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TLRDaDgdjVI/AAAAAAAAA-E/w2od5jXerx8/s1600-h/image%5B14%5D.png" ><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TLRDaqzZjBI/AAAAAAAAA-I/7iDtoy2U3Fc/image_thumb%5B6%5D.png?imgmax=800" alt="image" width="244" height="196" /></a>|<a href="http://lh5.ggpht.com/_5YPm4JGkfwE/TLRDayM9LUI/AAAAAAAAA-M/3ycTZyxeEkA/s1600-h/image%5B7%5D.png" ><img src="http://lh4.ggpht.com/_5YPm4JGkfwE/TLRDbe-2vLI/AAAAAAAAA-Q/CIoDmYlXnro/image_thumb%5B3%5D.png?imgmax=800" alt="image" width="332" height="130" /></a>

<a href="http://lh6.ggpht.com/_5YPm4JGkfwE/TLRDbyGy01I/AAAAAAAAA-U/IVyHb52EZ68/s1600-h/image%5B6%5D.png" ><img src="http://lh6.ggpht.com/_5YPm4JGkfwE/TLRDcTa0GEI/AAAAAAAAA-Y/Vidgg103loI/image_thumb%5B2%5D.png?imgmax=800" alt="image" width="325" height="126" /></a>|<a href="http://lh4.ggpht.com/_5YPm4JGkfwE/TLRDc7oQYcI/AAAAAAAAA-c/jdKoI7aFca8/s1600-h/image%5B11%5D.png" ><img src="http://lh5.ggpht.com/_5YPm4JGkfwE/TLRDdnPeo-I/AAAAAAAAA-g/tIcRuHgK-Ps/image_thumb%5B5%5D.png?imgmax=800" alt="image" width="306" height="133" /></a>

### TreelistView 
Enable recursive filtering and viewing for your listviews ,map XtraTreeList options to model, support for conditional appearance module.

Blogs:\
http://apobekiaris.blogspot.com/search/label/TreeListEditors

### NCarousel
Web skinable listeditor based on JCarousel. Image loading is asynchronous , multiple editor with different configurations is allowed.

<img src="http://www.expandframework.com/images/site/firstpage/ncarouseleditor.png" alt="ncarouseleditor" width="300" height="196" />

 Blogs:\
 http://apobekiaris.blogspot.com/search/label/NCaousel

### Workflow

Blogs:\
http://apobekiaris.blogspot.com/search/label/XpandWF

### ExcelImporter

![Windows ExcelImporter](http://www.expandframework.com/images/site/firstpage/excelimporter.win.png)

![Web ExcelImporter](http://www.expandframework.com/images/site/firstpage/excelimporter.web.png)

Blogs:\
http://apobekiaris.blogspot.gr/search/label/ExcelImporter


## Contributors

### Code Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)].
<a href="https://github.com/eXpandFramework/eXpand/graphs/contributors"><img src="https://opencollective.com/eXpand/contributors.svg?width=890&button=false" /></a>

### Financial Contributors

Become a financial contributor and help us sustain our community. [[Contribute](https://opencollective.com/eXpand/contribute)]

#### Individuals

<a href="https://opencollective.com/eXpand"><img src="https://opencollective.com/eXpand/individuals.svg?width=890"></a>

#### Organizations

Support this project with your organization. Your logo will show up here with a link to your website. [[Contribute](https://opencollective.com/eXpand/contribute)]

<a href="https://opencollective.com/eXpand/organization/0/website"><img src="https://opencollective.com/eXpand/organization/0/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/1/website"><img src="https://opencollective.com/eXpand/organization/1/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/2/website"><img src="https://opencollective.com/eXpand/organization/2/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/3/website"><img src="https://opencollective.com/eXpand/organization/3/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/4/website"><img src="https://opencollective.com/eXpand/organization/4/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/5/website"><img src="https://opencollective.com/eXpand/organization/5/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/6/website"><img src="https://opencollective.com/eXpand/organization/6/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/7/website"><img src="https://opencollective.com/eXpand/organization/7/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/8/website"><img src="https://opencollective.com/eXpand/organization/8/avatar.svg"></a>
<a href="https://opencollective.com/eXpand/organization/9/website"><img src="https://opencollective.com/eXpand/organization/9/avatar.svg"></a>
