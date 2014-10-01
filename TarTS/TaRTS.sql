USE [master]
GO

CREATE DATABASE [TarTS]
GO

USE [TarTS]
GO

CREATE TABLE [dbo].[CitizenData](
	[SSN] [varchar](11) NULL,
	[TaxID] [int] IDENTITY(1001,1) NOT NULL,
	[BirthDate] [datetime] NULL,
	[Salutation] [varchar](4) NULL,
	[Suffix] [nvarchar](4) NULL,
	[LastName] [varchar](80) NULL,
	[FirstName] [varchar](80) NULL,
	[MiddleInitial] [varchar](80) NULL,
	[ContactID] [int] NULL,
	[EmployerID] [int] NULL,
 CONSTRAINT [CitizenData_PK] PRIMARY KEY CLUSTERED 
(
	[TaxID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[ContactInformation](
	[ContactID] [int] IDENTITY(2001,1) NOT NULL,
	[TaxID] [int] NULL,
	[Address1] [varchar](80) NULL,
	[Address2] [varchar](80) NULL,
	[City] [varchar](80) NULL,
	[State] [varchar](2) NULL,
	[ZIP] [varchar](10) NULL,
 CONSTRAINT [ContactInformationData_PK] PRIMARY KEY CLUSTERED 
(
	[ContactID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[EmployerData](
	[EmployerID] [int] IDENTITY(3001,1) NOT NULL,
	[TaxID] [int] NULL,
	[EmployerName] [varchar](80) NULL,
	[ContactID] [int] NULL,
 CONSTRAINT [EmployerData_PK] PRIMARY KEY CLUSTERED 
(
	[EmployerID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[PhoneInformation](
	[PhoneID] [int] IDENTITY(4001,1) NOT NULL,
	[TaxID] [int] NULL,
	[PhoneNum] [varchar](12) NULL,
	[PhoneType] [varchar](20) NULL
 CONSTRAINT [PhoneInformation_PK] PRIMARY KEY CLUSTERED 
(
	[PhoneID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Resolution](
	[TaxID] [int] NULL,
	[ResolutionID] [int] IDENTITY(7001,1) NOT NULL,
	[PaymentID] [int] NULL,
	[DateResolved] [datetime] NULL,
	[ResolutionDescription] [varchar](500) NULL,
 CONSTRAINT [Resolution_PK] PRIMARY KEY CLUSTERED 
(
	[ResolutionID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[ReturnResolution](
	[ReturnID] [int] IDENTITY(6001,1) NOT NULL,
	[ResolutionID] [int] NOT NULL,
 CONSTRAINT [ReturnResolution_PK] PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC,
	[ResolutionID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)ON [PRIMARY]

GO

CREATE TABLE [dbo].[TaxReturnInformation](
	[TaxID] [int] NULL,
	[ReturnID] [int] IDENTITY(6001,1) NOT NULL,
	[FilingStatus] [varchar](50) NULL,
	[ReturnType] [varchar](10) NULL,
	[ReturnAmount] [money] NULL,
	[DateFiled] [datetime] NULL,
 CONSTRAINT [TaxReturnInformation_PK] PRIMARY KEY CLUSTERED 
(
	[ReturnID] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
)ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
