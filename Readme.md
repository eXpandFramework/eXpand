|  | Build | Nuget
|----------|--------|--------
Stable|[![Build status](https://dev.azure.com/eXpandDevOps/eXpandFramework/_apis/build/status/eXpandFramework-Release)](https://dev.azure.com/eXpandDevOps/eXpandFramework/_build/latest?definitionId=7)|https://api-v2v3search-0.nuget.org/query?q=eXpandFramework</br>OR</br> & .\nuget.exe list author:eXpandFramework
Lab|[![Build Status](https://dev.azure.com/eXpandDevOps/eXpandFramework/_apis/build/status/eXpandFramework-%20Nightly)](https://dev.azure.com/eXpandDevOps/eXpandFramework/_build/latest?definitionId=3)|https://xpandnugetserver.azurewebsites.net

# About
_eXpandFramework_ is the first open source project based on the _DevExpress eXpressApp Framework (XAF)_. The _eXpressApp Framework_ ships with _DXperience Universal_ and can be evaluated free of charge (30 day trial) at: <https://www.devexpress.com/ClientCenter/Downloads.>

_eXpandFramework_ was started and is led by [Tolis Bekiaris](http://apobekiaris.blogspot.com/). _Tolis_ has made a huge contribution to _XAF_ over the years receiving great recognition from _DevExpress_ and the whole _XAF_ community. In _April 2011 Tolis_ was honoured to be hired as **Technical Evangelist for DevExpress frameworks**!

The _eXpandFramework_ is an extension of XAF and operates within the **Microsoft Public License (Ms-PL)**. Although _XAF_ is not an open source product it can be purchased directly from DevExpress.

The _eXpandFramework_ team have extended the capabilities of the _eXpressApp Framework_ to include **70 cutting-edge assemblies** containing tools and modules that target numerous business scenarios.

The main idea _behind eXpandFramework_ is to offer as many features as possible to developers/business users  through a declarative approach (configuring files rather than writing code). Please go through each of the following brief descriptions and find out how _eXpandFramework_ can help you accomplish your complex development tasks with easy declarative programming. Below you can see some descriptions and screenshots of our basic modules (screenshots taken from eXpand FeatureCenter application that comes along with the download). In the folder _Demos_ you can find a list of demos like _XVideoRental_, _SecurityDemo_, _MiddleTier_, _Workflow_ and _installation helper solutions for each module_.

## Modules

Examples of those modules include (in the two right columns you can see the supported platform):

Module Name | Description | ![Windows](http://www.expandframework.com/images/site/windows.jpg "Windows") | ![ASPNET](http://www.expandframework.com/images/site/aspnet.jpg "ASPNET")
------------|-------------|---------|-------
[ModelDifference](#model-difference) | Model managment | Y | Y
[Dashboard](#dashboard) |    Enables native XAF dashboard collaboration and integrates the Dashboard suite |    Y |    Y
WorldCreator |  Design runtime assemblies | Y | Y
Email |     Send emails using bussiness rules from application model without coding (see <http://goo.gl/Hkx6PK)> |    Y|Y
JobSheduler    | Acts as a wrapper for the powerfull Quartz.Net, providing a flexible UI for managing Jobs at runtime |    Y |    Y
WorkFlow |    Contains workflow related features (Scheduled workflows) |    Y |    Y
DBMapper |    Map 14 different types of databases at runtime into worldcreator persistent objects    | Y |    Y
IO |    Export & Import object graphs |    Y |    Y
ExcelImporter |    Imports Excel, csv files. |    Y |    Y
MapView |     Google Maps integration for XAF web apps. Blog posts. |     Y |    Y
FileAttachments |     Provides support for file system storage as per E965 |    Y |    Y
Scheduler |     Please explore the XVideoRental module found in Demos/XVideoRental folder (Blog posts) |    Y |    Y
Reports |     Please explore the XVideoRental module found in Demos/XVideoRental folder    | Y |    N
Chart |      Please explore the XVideoRental module found in Demos/XVideoRental folder     |Y |    N
PivotGrid |      Please explore the XVideoRental module found in Demos/XVideoRental folder |    Y |    N
HtmlPropertyEditor |      File upload and configuration through Application Model |    N |    Y
Import Wizard |    Universal module for importing excel files into any XAF application.     |Y|    N
Core|    Support multiple datastore , calculable properties at runtime ,dynamic model creation,control grid options, datacaching, web master detail, view inheritance etc.    |Y|    Y
WorkFlow|    Extends  XAF's workflow module to support schedule and on object changed workflows|    Y|    Y
AuditTrail    |Configures XAF Audit Trail module using the Application Modules. (see Declarative data auditing)|    Y|     Y
StateMachine|    Enhance XAF's statemachine module in order to control transitions using permissions    |Y|    Y
Logic|    Define conditional architecture    |Y|    Y
ModelArtifact|    Parameterize model artifacts (Controllers, Actions, Views)|    Y|    Y
AdditionalViewControlsProvider    |Decorate your views with custom controls|    Y    |Y
MasterDetail|    XtraGrid support for master-detail data presentation using the model.    Y|    N|
PivotChart|    Enhance analysis procedures / controls|    Y|    Y
Security|    Provides extension methods, authentication providers, login remember me, custom security objects|     Y|    Y|
MemberLevelSecurity|    Conditional security for object members.|    Y|    N
FilterDatastore|    Filter data at low level|    Y    |Y
Wizard|    Design wizard driven views|    Y    |N
ViewVariants|    Create views without the use of model editor|    Y|    Y
Validation|    More rules , permission validation, warning/info support, Action contexts etc|    Y|    Y
ConditionalObjectViews|    Allows the conditional navigation to your detailviews/listviews-->Merged with ModelArtifact    |Y|    Y
EasyTests|    Custom command and extensions for EasyTest see <http://apobekiaris.blogspot.gr/search/label/EasyTest>|    Y|    Y
TreelistView|    Enhance hierarchy controls, map XtraTreeList options to model|    Y|    Y
NCarousel|    Loads images asynchronously and displays them using a configurable carousel listeditor|    N|    Y
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
