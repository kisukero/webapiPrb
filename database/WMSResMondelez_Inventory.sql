USE [WMSResMondelez];
GO
IF OBJECT_ID('dbo.InventoryUsers', 'U') IS NULL
CREATE TABLE dbo.InventoryUsers (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, UserName NVARCHAR(50) NOT NULL, FullName NVARCHAR(150) NOT NULL, PasswordHash NVARCHAR(500) NOT NULL, IsActive BIT NOT NULL CONSTRAINT DF_InventoryUsers_IsActive DEFAULT 1, CONSTRAINT UQ_InventoryUsers_UserName UNIQUE (UserName));
GO
IF OBJECT_ID('dbo.InventoryProducts', 'U') IS NULL
CREATE TABLE dbo.InventoryProducts (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, Code NVARCHAR(50) NOT NULL, Name NVARCHAR(150) NOT NULL, Description NVARCHAR(500) NULL, IsActive BIT NOT NULL CONSTRAINT DF_InventoryProducts_IsActive DEFAULT 1, CONSTRAINT UQ_InventoryProducts_Code UNIQUE (Code));
GO
IF OBJECT_ID('dbo.InventoryProductLots', 'U') IS NULL
CREATE TABLE dbo.InventoryProductLots (Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY, LotNumber NVARCHAR(50) NOT NULL, Supplier NVARCHAR(150) NOT NULL, Price DECIMAL(18,2) NOT NULL, Stock INT NOT NULL, ProductId INT NOT NULL, CONSTRAINT CK_InventoryProductLots_Price CHECK (Price >= 0), CONSTRAINT CK_InventoryProductLots_Stock CHECK (Stock >= 0), CONSTRAINT FK_InventoryProductLots_Product FOREIGN KEY (ProductId) REFERENCES dbo.InventoryProducts(Id) ON DELETE CASCADE, CONSTRAINT UQ_InventoryProductLots_Product_Lot_Supplier UNIQUE (ProductId, LotNumber, Supplier));
GO
