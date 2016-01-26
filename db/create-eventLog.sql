
CREATE TABLE [dbo].[EventLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventDate] [datetime] NOT NULL,
	[Thread] [varchar](255) NOT NULL,
	[Level] [varchar](50) NOT NULL,
	[Logger] [varchar](255) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Exception] [varchar](max) NULL,
	CONSTRAINT [PK_EventLog] PRIMARY KEY CLUSTERED 
		( [Id] ASC ) ON [PRIMARY]
)

-- sqlite
--CREATE TABLE EventLog (
--    Id           INTEGER PRIMARY KEY,
--    EventDate    DATETIME NOT NULL,
--    Thread       VARCHAR(255) NOT NULL,
--    Level        VARCHAR(50) NOT NULL,
--    Logger       VARCHAR(255) NOT NULL,
--    Message      TEXT DEFAULT NULL,
--    Exception    TEXT DEFAULT NULL
--);