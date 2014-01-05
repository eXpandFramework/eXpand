SET QUOTED_IDENTIFIER OFF;
GO
USE [EFDemo_v13.2];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NULL,
    [Office] nvarchar(max)  NULL
);
GO

-- Creating table 'Positions'
CREATE TABLE [dbo].[Positions] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max)  NULL
);
GO

-- Creating table 'Parties'
CREATE TABLE [dbo].[Parties] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Photo_Binary] varbinary(max)  NULL,
    [Address1_ID] int  NULL,
    [Address2_ID] int  NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DateCompleted] datetime  NULL,
    [Subject] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [DueDate] datetime  NULL,
    [StartDate] datetime  NULL,
    [Status_Int] int  NOT NULL,
    [PercentCompleted] int  NOT NULL,
    [AssignedTo_ID] int  NULL
);
GO

-- Creating table 'Notes'
CREATE TABLE [dbo].[Notes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Author] nvarchar(max)  NULL,
    [DateTime] datetime  NULL,
    [Text] nvarchar(max)  NULL
);
GO

-- Creating table 'PhoneNumbers'
CREATE TABLE [dbo].[PhoneNumbers] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Number] nvarchar(max)  NULL,
    [PhoneType] nvarchar(max)  NULL,
    [Party_ID] int  NULL
);
GO

-- Creating table 'States'
CREATE TABLE [dbo].[States] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ShortName] nvarchar(max)  NULL,
    [LongName] nvarchar(max)  NULL
);
GO

-- Creating table 'Payments'
CREATE TABLE [dbo].[Payments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Rate] decimal(18,0)  NOT NULL,
    [Hours] decimal(18,0)  NOT NULL
);
GO

-- Creating table 'Countries'
CREATE TABLE [dbo].[Countries] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [PhoneCode] nvarchar(max)  NULL
);
GO

-- Creating table 'HCategories'
CREATE TABLE [dbo].[HCategories] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Parent_ID] int  NULL
);
GO

-- Creating table 'Addresses'
CREATE TABLE [dbo].[Addresses] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Street] nvarchar(max)  NULL,
    [City] nvarchar(max)  NULL,
    [StateProvince] nvarchar(max)  NULL,
    [ZipPostal] nvarchar(max)  NULL,
    [Country_ID] int  NULL
);
GO

-- Creating table 'Resumes'
CREATE TABLE [dbo].[Resumes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Contact_ID] int  NULL
);
GO

-- Creating table 'Events'
CREATE TABLE [dbo].[Events] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Subject] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [StartOn] datetime  NULL,
    [EndOn] datetime  NULL,
    [AllDay] bit  NOT NULL,
    [Location] nvarchar(max)  NULL,
    [Label] int  NOT NULL,
    [Status] int  NOT NULL,
    [Type] int  NOT NULL,
    [RecurrenceInfoXml] nvarchar(max)  NULL,
    [RecurrencePattern_ID] int  NULL
);
GO

-- Creating table 'Resources'
CREATE TABLE [dbo].[Resources] (
    [Key] int IDENTITY(1,1) NOT NULL,
    [Caption] nvarchar(max)  NULL,
    [Color_Int] int  NOT NULL
);
GO

-- Creating table 'FileData'
CREATE TABLE [dbo].[FileData] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Size] int  NOT NULL,
    [FileName] nvarchar(max)  NULL,
    [Content] varbinary(max)  NULL
);
GO

-- Creating table 'FileAttachments'
CREATE TABLE [dbo].[FileAttachments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [File_ID] int  NOT NULL
);
GO

-- Creating table 'ReportData'
CREATE TABLE [dbo].[ReportData] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Content] varbinary(max)  NULL,
    [DataTypeName] nvarchar(max)  NULL,
    [IsInplaceReport] bit  NOT NULL,
    [ReportName] nvarchar(max)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [IsAdministrative] bit  NOT NULL,
    [CanEditModel] bit  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NULL,
    [IsActive] bit  NOT NULL,
    [ChangePasswordOnFirstLogon] bit  NOT NULL,
    [StoredPassword] nvarchar(max)  NULL
);
GO

-- Creating table 'TypePermissionObjects'
CREATE TABLE [dbo].[TypePermissionObjects] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [AllowRead] bit  NOT NULL,
    [AllowWrite] bit  NOT NULL,
    [AllowCreate] bit  NOT NULL,
    [AllowDelete] bit  NOT NULL,
    [AllowNavigate] bit  NOT NULL,
    [TargetTypeFullName] nvarchar(max)  NOT NULL,
    [Role_ID] int  NULL
);
GO

-- Creating table 'ModulesInfo'
CREATE TABLE [dbo].[ModulesInfo] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Version] nvarchar(max)  NULL,
    [AssemblyFileName] nvarchar(max)  NULL,
    [IsMain] bit  NOT NULL
);
GO

-- Creating table 'Analysis'
CREATE TABLE [dbo].[Analysis] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Criteria] nvarchar(max)  NULL,
    [ObjectTypeName] nvarchar(max)  NULL,
    [DimensionPropertiesString] nvarchar(max)  NULL,
    [PivotGridSettingsContent] varbinary(max)  NULL,
    [ChartSettingsContent] varbinary(max)  NULL
);
GO

-- Creating table 'Parties_Person'
CREATE TABLE [dbo].[Parties_Person] (
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NULL,
    [MiddleName] nvarchar(max)  NULL,
    [Birthday] datetime  NULL,
    [Email] nvarchar(max)  NULL,
    [ID] int  NOT NULL
);
GO

-- Creating table 'Parties_Contact'
CREATE TABLE [dbo].[Parties_Contact] (
    [WebPageAddress] nvarchar(max)  NULL,
    [NickName] nvarchar(max)  NULL,
    [SpouseName] nvarchar(max)  NULL,
    [TitleOfCourtesy_Int] int  NOT NULL,
    [Anniversary] datetime  NULL,
    [Notes] nvarchar(max)  NULL,
    [ID] int  NOT NULL,
    [Department_ID] int  NULL,
    [Position_ID] int  NULL,
    [Manager_ID] int  NULL
);
GO

-- Creating table 'FileAttachments_PortfolioFileData'
CREATE TABLE [dbo].[FileAttachments_PortfolioFileData] (
    [DocumentType_Int] int  NOT NULL,
    [ID] int  NOT NULL,
    [Resume_ID] int  NOT NULL
);
GO

-- Creating table 'Tasks_DemoTask'
CREATE TABLE [dbo].[Tasks_DemoTask] (
    [Priority_Int] int  NOT NULL,
    [EstimatedWork] int  NULL,
    [ActualWork] int  NULL,
    [ID] int  NOT NULL
);
GO

-- Creating table 'Parties_Organization'
CREATE TABLE [dbo].[Parties_Organization] (
    [FullName] nvarchar(max)  NULL,
    [Profile] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [WebSite] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [Name] nvarchar(max)  NULL,
    [ID] int  NOT NULL
);
GO

-- Creating table 'Departments_Positions'
CREATE TABLE [dbo].[Departments_Positions] (
    [Departments_ID] int  NOT NULL,
    [Positions_ID] int  NOT NULL
);
GO

-- Creating table 'Events_Resources'
CREATE TABLE [dbo].[Events_Resources] (
    [Events_ID] int  NOT NULL,
    [Resources_Key] int  NOT NULL
);
GO

-- Creating table 'Users_Roles'
CREATE TABLE [dbo].[Users_Roles] (
    [Users_ID] int  NOT NULL,
    [Roles_ID] int  NOT NULL
);
GO

-- Creating table 'Roles_Roles'
CREATE TABLE [dbo].[Roles_Roles] (
    [ParentRoles_ID] int  NOT NULL,
    [ChildRoles_ID] int  NOT NULL
);
GO

-- Creating table 'DemoTasks_Contacts'
CREATE TABLE [dbo].[DemoTasks_Contacts] (
    [Tasks_ID] int  NOT NULL,
    [Contacts_ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Positions'
ALTER TABLE [dbo].[Positions]
ADD CONSTRAINT [PK_Positions]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Parties'
ALTER TABLE [dbo].[Parties]
ADD CONSTRAINT [PK_Parties]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Notes'
ALTER TABLE [dbo].[Notes]
ADD CONSTRAINT [PK_Notes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PhoneNumbers'
ALTER TABLE [dbo].[PhoneNumbers]
ADD CONSTRAINT [PK_PhoneNumbers]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [PK_States]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Payments'
ALTER TABLE [dbo].[Payments]
ADD CONSTRAINT [PK_Payments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Countries'
ALTER TABLE [dbo].[Countries]
ADD CONSTRAINT [PK_Countries]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'HCategories'
ALTER TABLE [dbo].[HCategories]
ADD CONSTRAINT [PK_HCategories]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [PK_Addresses]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Resumes'
ALTER TABLE [dbo].[Resumes]
ADD CONSTRAINT [PK_Resumes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [PK_Events]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Key] in table 'Resources'
ALTER TABLE [dbo].[Resources]
ADD CONSTRAINT [PK_Resources]
    PRIMARY KEY CLUSTERED ([Key] ASC);
GO

-- Creating primary key on [ID] in table 'FileData'
ALTER TABLE [dbo].[FileData]
ADD CONSTRAINT [PK_FileData]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'FileAttachments'
ALTER TABLE [dbo].[FileAttachments]
ADD CONSTRAINT [PK_FileAttachments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ReportData'
ALTER TABLE [dbo].[ReportData]
ADD CONSTRAINT [PK_ReportData]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'TypePermissionObjects'
ALTER TABLE [dbo].[TypePermissionObjects]
ADD CONSTRAINT [PK_TypePermissionObjects]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'ModulesInfo'
ALTER TABLE [dbo].[ModulesInfo]
ADD CONSTRAINT [PK_ModulesInfo]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Analysis'
ALTER TABLE [dbo].[Analysis]
ADD CONSTRAINT [PK_Analysis]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Parties_Person'
ALTER TABLE [dbo].[Parties_Person]
ADD CONSTRAINT [PK_Parties_Person]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Parties_Contact'
ALTER TABLE [dbo].[Parties_Contact]
ADD CONSTRAINT [PK_Parties_Contact]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'FileAttachments_PortfolioFileData'
ALTER TABLE [dbo].[FileAttachments_PortfolioFileData]
ADD CONSTRAINT [PK_FileAttachments_PortfolioFileData]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Tasks_DemoTask'
ALTER TABLE [dbo].[Tasks_DemoTask]
ADD CONSTRAINT [PK_Tasks_DemoTask]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Parties_Organization'
ALTER TABLE [dbo].[Parties_Organization]
ADD CONSTRAINT [PK_Parties_Organization]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Departments_ID], [Positions_ID] in table 'Departments_Positions'
ALTER TABLE [dbo].[Departments_Positions]
ADD CONSTRAINT [PK_Departments_Positions]
    PRIMARY KEY NONCLUSTERED ([Departments_ID], [Positions_ID] ASC);
GO

-- Creating primary key on [Events_ID], [Resources_Key] in table 'Events_Resources'
ALTER TABLE [dbo].[Events_Resources]
ADD CONSTRAINT [PK_Events_Resources]
    PRIMARY KEY NONCLUSTERED ([Events_ID], [Resources_Key] ASC);
GO

-- Creating primary key on [Users_ID], [Roles_ID] in table 'Users_Roles'
ALTER TABLE [dbo].[Users_Roles]
ADD CONSTRAINT [PK_Users_Roles]
    PRIMARY KEY NONCLUSTERED ([Users_ID], [Roles_ID] ASC);
GO

-- Creating primary key on [ParentRoles_ID], [ChildRoles_ID] in table 'Roles_Roles'
ALTER TABLE [dbo].[Roles_Roles]
ADD CONSTRAINT [PK_Roles_Roles]
    PRIMARY KEY NONCLUSTERED ([ParentRoles_ID], [ChildRoles_ID] ASC);
GO

-- Creating primary key on [Tasks_ID], [Contacts_ID] in table 'DemoTasks_Contacts'
ALTER TABLE [dbo].[DemoTasks_Contacts]
ADD CONSTRAINT [PK_DemoTasks_Contacts]
    PRIMARY KEY NONCLUSTERED ([Tasks_ID], [Contacts_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Departments_ID] in table 'Departments_Positions'
ALTER TABLE [dbo].[Departments_Positions]
ADD CONSTRAINT [FK_Departments_Positions_Department]
    FOREIGN KEY ([Departments_ID])
    REFERENCES [dbo].[Departments]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Positions_ID] in table 'Departments_Positions'
ALTER TABLE [dbo].[Departments_Positions]
ADD CONSTRAINT [FK_Departments_Positions_Position]
    FOREIGN KEY ([Positions_ID])
    REFERENCES [dbo].[Positions]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Departments_Positions_Position'
CREATE INDEX [IX_FK_Departments_Positions_Position]
ON [dbo].[Departments_Positions]
    ([Positions_ID]);
GO

-- Creating foreign key on [Department_ID] in table 'Parties_Contact'
ALTER TABLE [dbo].[Parties_Contact]
ADD CONSTRAINT [FK_Contact_Department]
    FOREIGN KEY ([Department_ID])
    REFERENCES [dbo].[Departments]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Contact_Department'
CREATE INDEX [IX_FK_Contact_Department]
ON [dbo].[Parties_Contact]
    ([Department_ID]);
GO

-- Creating foreign key on [Position_ID] in table 'Parties_Contact'
ALTER TABLE [dbo].[Parties_Contact]
ADD CONSTRAINT [FK_Contact_Position]
    FOREIGN KEY ([Position_ID])
    REFERENCES [dbo].[Positions]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Contact_Position'
CREATE INDEX [IX_FK_Contact_Position]
ON [dbo].[Parties_Contact]
    ([Position_ID]);
GO

-- Creating foreign key on [Party_ID] in table 'PhoneNumbers'
ALTER TABLE [dbo].[PhoneNumbers]
ADD CONSTRAINT [FK_PhoneNumber_Party]
    FOREIGN KEY ([Party_ID])
    REFERENCES [dbo].[Parties]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PhoneNumber_Party'
CREATE INDEX [IX_FK_PhoneNumber_Party]
ON [dbo].[PhoneNumbers]
    ([Party_ID]);
GO

-- Creating foreign key on [Parent_ID] in table 'HCategories'
ALTER TABLE [dbo].[HCategories]
ADD CONSTRAINT [FK_HCategory_HCategory]
    FOREIGN KEY ([Parent_ID])
    REFERENCES [dbo].[HCategories]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HCategory_HCategory'
CREATE INDEX [IX_FK_HCategory_HCategory]
ON [dbo].[HCategories]
    ([Parent_ID]);
GO

-- Creating foreign key on [Country_ID] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [FK_Address_Country]
    FOREIGN KEY ([Country_ID])
    REFERENCES [dbo].[Countries]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Address_Country'
CREATE INDEX [IX_FK_Address_Country]
ON [dbo].[Addresses]
    ([Country_ID]);
GO

-- Creating foreign key on [Events_ID] in table 'Events_Resources'
ALTER TABLE [dbo].[Events_Resources]
ADD CONSTRAINT [FK_Events_Resources_Event]
    FOREIGN KEY ([Events_ID])
    REFERENCES [dbo].[Events]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Resources_Key] in table 'Events_Resources'
ALTER TABLE [dbo].[Events_Resources]
ADD CONSTRAINT [FK_Events_Resources_Resource]
    FOREIGN KEY ([Resources_Key])
    REFERENCES [dbo].[Resources]
        ([Key])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Events_Resources_Resource'
CREATE INDEX [IX_FK_Events_Resources_Resource]
ON [dbo].[Events_Resources]
    ([Resources_Key]);
GO

-- Creating foreign key on [File_ID] in table 'FileAttachments'
ALTER TABLE [dbo].[FileAttachments]
ADD CONSTRAINT [FK_FileAttachment_FileData]
    FOREIGN KEY ([File_ID])
    REFERENCES [dbo].[FileData]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FileAttachment_FileData'
CREATE INDEX [IX_FK_FileAttachment_FileData]
ON [dbo].[FileAttachments]
    ([File_ID]);
GO

-- Creating foreign key on [Resume_ID] in table 'FileAttachments_PortfolioFileData'
ALTER TABLE [dbo].[FileAttachments_PortfolioFileData]
ADD CONSTRAINT [FK_PortfolioFileData_Resume]
    FOREIGN KEY ([Resume_ID])
    REFERENCES [dbo].[Resumes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PortfolioFileData_Resume'
CREATE INDEX [IX_FK_PortfolioFileData_Resume]
ON [dbo].[FileAttachments_PortfolioFileData]
    ([Resume_ID]);
GO

-- Creating foreign key on [Contact_ID] in table 'Resumes'
ALTER TABLE [dbo].[Resumes]
ADD CONSTRAINT [FK_Resume_Contact]
    FOREIGN KEY ([Contact_ID])
    REFERENCES [dbo].[Parties_Contact]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Resume_Contact'
CREATE INDEX [IX_FK_Resume_Contact]
ON [dbo].[Resumes]
    ([Contact_ID]);
GO

-- Creating foreign key on [Address1_ID] in table 'Parties'
ALTER TABLE [dbo].[Parties]
ADD CONSTRAINT [FK_Party_Address1]
    FOREIGN KEY ([Address1_ID])
    REFERENCES [dbo].[Addresses]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Party_Address1'
CREATE INDEX [IX_FK_Party_Address1]
ON [dbo].[Parties]
    ([Address1_ID]);
GO

-- Creating foreign key on [Address2_ID] in table 'Parties'
ALTER TABLE [dbo].[Parties]
ADD CONSTRAINT [FK_Party_Address2]
    FOREIGN KEY ([Address2_ID])
    REFERENCES [dbo].[Addresses]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Party_Address2'
CREATE INDEX [IX_FK_Party_Address2]
ON [dbo].[Parties]
    ([Address2_ID]);
GO

-- Creating foreign key on [Manager_ID] in table 'Parties_Contact'
ALTER TABLE [dbo].[Parties_Contact]
ADD CONSTRAINT [FK_Contact_Contact]
    FOREIGN KEY ([Manager_ID])
    REFERENCES [dbo].[Parties_Contact]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Contact_Contact'
CREATE INDEX [IX_FK_Contact_Contact]
ON [dbo].[Parties_Contact]
    ([Manager_ID]);
GO

-- Creating foreign key on [Users_ID] in table 'Users_Roles'
ALTER TABLE [dbo].[Users_Roles]
ADD CONSTRAINT [FK_Users_Roles_User]
    FOREIGN KEY ([Users_ID])
    REFERENCES [dbo].[Users]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Roles_ID] in table 'Users_Roles'
ALTER TABLE [dbo].[Users_Roles]
ADD CONSTRAINT [FK_Users_Roles_Role]
    FOREIGN KEY ([Roles_ID])
    REFERENCES [dbo].[Roles]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Roles_Role'
CREATE INDEX [IX_FK_Users_Roles_Role]
ON [dbo].[Users_Roles]
    ([Roles_ID]);
GO

-- Creating foreign key on [Role_ID] in table 'TypePermissionObjects'
ALTER TABLE [dbo].[TypePermissionObjects]
ADD CONSTRAINT [FK_Role_TypePermissionObject]
    FOREIGN KEY ([Role_ID])
    REFERENCES [dbo].[Roles]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Role_TypePermissionObject'
CREATE INDEX [IX_FK_Role_TypePermissionObject]
ON [dbo].[TypePermissionObjects]
    ([Role_ID]);
GO

-- Creating foreign key on [ParentRoles_ID] in table 'Roles_Roles'
ALTER TABLE [dbo].[Roles_Roles]
ADD CONSTRAINT [FK_Roles_Roles_Role1]
    FOREIGN KEY ([ParentRoles_ID])
    REFERENCES [dbo].[Roles]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ChildRoles_ID] in table 'Roles_Roles'
ALTER TABLE [dbo].[Roles_Roles]
ADD CONSTRAINT [FK_Roles_Roles_Role2]
    FOREIGN KEY ([ChildRoles_ID])
    REFERENCES [dbo].[Roles]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Roles_Roles_Role2'
CREATE INDEX [IX_FK_Roles_Roles_Role2]
ON [dbo].[Roles_Roles]
    ([ChildRoles_ID]);
GO

-- Creating foreign key on [AssignedTo_ID] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_Task_Party]
    FOREIGN KEY ([AssignedTo_ID])
    REFERENCES [dbo].[Parties]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Task_Party'
CREATE INDEX [IX_FK_Task_Party]
ON [dbo].[Tasks]
    ([AssignedTo_ID]);
GO

-- Creating foreign key on [Tasks_ID] in table 'DemoTasks_Contacts'
ALTER TABLE [dbo].[DemoTasks_Contacts]
ADD CONSTRAINT [FK_DemoTasks_Contacts_DemoTask]
    FOREIGN KEY ([Tasks_ID])
    REFERENCES [dbo].[Tasks_DemoTask]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Contacts_ID] in table 'DemoTasks_Contacts'
ALTER TABLE [dbo].[DemoTasks_Contacts]
ADD CONSTRAINT [FK_DemoTasks_Contacts_Contact]
    FOREIGN KEY ([Contacts_ID])
    REFERENCES [dbo].[Parties_Contact]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DemoTasks_Contacts_Contact'
CREATE INDEX [IX_FK_DemoTasks_Contacts_Contact]
ON [dbo].[DemoTasks_Contacts]
    ([Contacts_ID]);
GO

-- Creating foreign key on [RecurrencePattern_ID] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [FK_Event_Event]
    FOREIGN KEY ([RecurrencePattern_ID])
    REFERENCES [dbo].[Events]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Event_Event'
CREATE INDEX [IX_FK_Event_Event]
ON [dbo].[Events]
    ([RecurrencePattern_ID]);
GO

-- Creating foreign key on [ID] in table 'Parties_Person'
ALTER TABLE [dbo].[Parties_Person]
ADD CONSTRAINT [FK_Person_inherits_Party]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[Parties]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ID] in table 'Parties_Contact'
ALTER TABLE [dbo].[Parties_Contact]
ADD CONSTRAINT [FK_Contact_inherits_Person]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[Parties_Person]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ID] in table 'FileAttachments_PortfolioFileData'
ALTER TABLE [dbo].[FileAttachments_PortfolioFileData]
ADD CONSTRAINT [FK_PortfolioFileData_inherits_FileAttachment]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[FileAttachments]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ID] in table 'Tasks_DemoTask'
ALTER TABLE [dbo].[Tasks_DemoTask]
ADD CONSTRAINT [FK_DemoTask_inherits_Task]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[Tasks]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ID] in table 'Parties_Organization'
ALTER TABLE [dbo].[Parties_Organization]
ADD CONSTRAINT [FK_Organization_inherits_Party]
    FOREIGN KEY ([ID])
    REFERENCES [dbo].[Parties]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------