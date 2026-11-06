USE [prb_BG_SQL];
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_User_Get
    @UserName NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserName, FullName, PasswordHash, IsActive
    FROM dbo.InventoryUsers
    WHERE UserName = @UserName AND IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_Product_List
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, Description, IsActive
    FROM dbo.InventoryProducts
    ORDER BY Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_Product_Get
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Code, Name, Description, IsActive
    FROM dbo.InventoryProducts
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_ProductLot_List
    @ProductId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, LotNumber, Supplier, Price, Stock, ProductId
    FROM dbo.InventoryProductLots
    WHERE @ProductId IS NULL OR ProductId = @ProductId
    ORDER BY ProductId, LotNumber, Supplier;
END
GO

-- @LotsJson recibe el arreglo lots que envía Angular.
CREATE OR ALTER PROCEDURE dbo.csp_Inventory_Product_Create
    @Code NVARCHAR(50),
    @Name NVARCHAR(150),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT,
    @LotsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF EXISTS (SELECT 1 FROM dbo.InventoryProducts WHERE Code = @Code)
        THROW 50001, 'El código de producto ya existe.', 1;
    IF ISJSON(@LotsJson) <> 1 OR NOT EXISTS (SELECT 1 FROM OPENJSON(@LotsJson))
        THROW 50002, 'Debe registrar al menos un lote.', 1;
    BEGIN TRANSACTION;
    INSERT dbo.InventoryProducts(Code, Name, Description, IsActive)
    VALUES (@Code, @Name, @Description, @IsActive);
    DECLARE @ProductId INT = SCOPE_IDENTITY();
    INSERT dbo.InventoryProductLots(LotNumber, Supplier, Price, Stock, ProductId)
    SELECT LotNumber, Supplier, Price, Stock, @ProductId
    FROM OPENJSON(@LotsJson)
    WITH (LotNumber NVARCHAR(50) '$.lotNumber', Supplier NVARCHAR(150) '$.supplier', Price DECIMAL(18,2) '$.price', Stock INT '$.stock');
    COMMIT TRANSACTION;
    EXEC dbo.csp_Inventory_Product_Get @ProductId;
END
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_Product_Update
    @Id INT,
    @Code NVARCHAR(50),
    @Name NVARCHAR(150),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT,
    @LotsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.InventoryProducts WHERE Id = @Id)
        THROW 50003, 'Producto no encontrado.', 1;
    IF EXISTS (SELECT 1 FROM dbo.InventoryProducts WHERE Code = @Code AND Id <> @Id)
        THROW 50001, 'El código de producto ya existe.', 1;
    IF ISJSON(@LotsJson) <> 1 OR NOT EXISTS (SELECT 1 FROM OPENJSON(@LotsJson))
        THROW 50002, 'Debe registrar al menos un lote.', 1;
    BEGIN TRANSACTION;
    UPDATE dbo.InventoryProducts SET Code=@Code, Name=@Name, Description=@Description, IsActive=@IsActive WHERE Id=@Id;
    DELETE FROM dbo.InventoryProductLots WHERE ProductId=@Id;
    INSERT dbo.InventoryProductLots(LotNumber, Supplier, Price, Stock, ProductId)
    SELECT LotNumber, Supplier, Price, Stock, @Id
    FROM OPENJSON(@LotsJson)
    WITH (LotNumber NVARCHAR(50) '$.lotNumber', Supplier NVARCHAR(150) '$.supplier', Price DECIMAL(18,2) '$.price', Stock INT '$.stock');
    COMMIT TRANSACTION;
END
GO

CREATE OR ALTER PROCEDURE dbo.csp_Inventory_Product_Delete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM dbo.InventoryProducts WHERE Id = @Id)
        THROW 50003, 'Producto no encontrado.', 1;
    DELETE FROM dbo.InventoryProducts WHERE Id = @Id;
END
GO
