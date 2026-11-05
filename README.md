# API de inventario

API mínima para el challenge: autenticación JWT, consulta de sesión y CRUD de productos. Un producto contiene varios lotes/proveedores, cada uno con precio y stock.

La solución está separada como en LMS: `Domain/Entities` y `Domain/Requests`; `Service/Data`, `Service/Interfaces` y `Service/Repositories`; `Business/Interfaces` y `Business/Services`. `webapi` deja solamente configuración, JWT y los dos controladores.

## Preparación

1. Ajusta `ConnectionStrings:WmsResMondelez` en `webapi/appsettings.json` para tu SQL Server. La base inicial es `WMSResMondelez`.
2. Ejecuta `database/WMSResMondelez_Inventory.sql` y luego `database/WMSResMondelez_Inventory_StoredProcedures.sql` en esa base.
3. Cambia `Jwt:Key` antes de publicar.
4. Ejecuta `dotnet restore` y `dotnet run --project webapi/webapi.csproj`.
5. Abre `/swagger`. CORS permite por defecto `http://localhost:4200`.

Al iniciar, si no existen usuarios, la API crea `admin` / `Admin123*`. Es solo para la prueba: cámbialo antes de publicar.

## Endpoints

| Método | Ruta | Uso |
| --- | --- | --- |
| POST | `/api/Auth/login` | Login, devuelve JWT |
| GET | `/api/Auth/session` | Verifica la sesión actual |
| GET | `/api/Products` | Lista productos y lotes |
| GET | `/api/Products/{id}` | Obtiene un producto |
| POST | `/api/Products` | Crea un producto |
| PUT | `/api/Products/{id}` | Actualiza producto y sus lotes |
| DELETE | `/api/Products/{id}` | Elimina producto y lotes |

Excepto `login`, todos requieren `Authorization: Bearer {token}`. En Swagger usa **Authorize** e ingresa `Bearer {token}`.

```json
{
  "code": "MON-001", "name": "Monitor 50 pulgadas 4K", "description": "Producto de prueba", "isActive": true,
  "lots": [
    { "lotNumber": "LOTE-A", "supplier": "Proveedor A", "price": 250.00, "stock": 8 },
    { "lotNumber": "LOTE-B", "supplier": "Proveedor B", "price": 300.00, "stock": 4 }
  ]
}
```

Los logs se guardan en `webapi/Logs/` y también se muestran por consola. Se manejan errores de validación, credenciales, conflicto de código y producto inexistente.

## Procedimientos almacenados

El segundo script crea `csp_Inventory_User_Get`, consultas de productos/lotes y los procedimientos `csp_Inventory_Product_Create`, `csp_Inventory_Product_Update` y `csp_Inventory_Product_Delete`. Los procedimientos de guardar reciben los lotes como JSON, igual que los envía Angular.
