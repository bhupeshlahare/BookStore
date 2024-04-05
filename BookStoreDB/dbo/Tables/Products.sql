CREATE TABLE [dbo].[Products] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [ISBN]        NVARCHAR (MAX) NOT NULL,
    [Author]      NVARCHAR (MAX) NOT NULL,
    [ListPrice]   FLOAT (53)     NOT NULL,
    [Price]       FLOAT (53)     NOT NULL,
    [Price50]     FLOAT (53)     NOT NULL,
    [Price100]    FLOAT (53)     NOT NULL,
    [CategoryId]  INT            DEFAULT ((0)) NOT NULL,
    [ImageUrl]    NVARCHAR (MAX) DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId]
    ON [dbo].[Products]([CategoryId] ASC);

