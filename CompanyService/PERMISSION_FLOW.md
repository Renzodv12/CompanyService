# Flujo para Agregar Permisos en CompanyService

## Estructura del Sistema de Permisos

### 1. Entidades Principales

#### Permission (Permiso)

```csharp
public class Permission
{
    public Guid Id { get; set; }
    public string Key { get; set; }        // Ej: "Company", "User", "Role"
    public string Description { get; set; } // Descripción del permiso
    public ICollection<RolePermission> RolePermissions { get; set; }
}
```

#### RolePermission (Relación Rol-Permiso)

```csharp
public class RolePermission
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; }
    public PermissionAction Actions { get; set; } = PermissionAction.None;
}
```

#### PermissionAction (Acciones Disponibles)

```csharp
public enum PermissionAction
{
    None = 0,
    View = 1,      // Ver/Leer
    Create = 2,    // Crear
    Edit = 4,      // Editar/Modificar
    Delete = 8     // Eliminar
}
```

## Flujo Completo para Agregar Permisos

### Paso 1: Crear un Nuevo Permiso

**Endpoint:** `POST /api/permissions`

**Request Body:**

```json
{
  "key": "Product",
  "description": "Permisos para gestión de productos"
}
```

**Proceso:**

1. Validar que no exista un permiso con la misma clave
2. Crear nueva entidad Permission
3. Guardar en base de datos
4. Retornar PermissionDto

**Response:**

```json
{
  "key": "Product",
  "description": "Permisos para gestión de productos",
  "actions": []
}
```

### Paso 2: Asignar Permisos a un Rol

**Endpoint:** `POST /api/roles/{roleId}/permissions`

**Request Body:**

```json
{
  "permissionIds": [
    "guid-del-permiso-1",
    "guid-del-permiso-2",
    "guid-del-permiso-3"
  ]
}
```

**Proceso:**

1. Verificar que el rol existe
2. Verificar que todos los permisos existen
3. Eliminar permisos actuales del rol
4. Crear nuevas relaciones RolePermission
5. Guardar cambios

### Paso 3: Configurar Acciones Específicas (Opcional)

**Nota:** Actualmente el sistema no tiene endpoints específicos para configurar acciones, pero la estructura está preparada para ello.

**Estructura de RolePermission con acciones:**

```csharp
var rolePermission = new RolePermission
{
    RoleId = roleId,
    PermissionId = permissionId,
    Actions = PermissionAction.View | PermissionAction.Create | PermissionAction.Edit
};
```

## Endpoints Disponibles

### Gestión de Permisos

- `GET /api/permissions` - Obtener todos los permisos
- `GET /api/permissions/{id}` - Obtener permiso por ID
- `POST /api/permissions` - Crear nuevo permiso
- `PUT /api/permissions/{id}` - Actualizar permiso
- `DELETE /api/permissions/{id}` - Eliminar permiso
- `GET /api/permissions/{id}/roles` - Obtener roles que tienen este permiso

### Gestión de Roles y Permisos

- `POST /api/roles/{id}/permissions` - Asignar permisos a rol
- `DELETE /api/roles/{roleId}/permissions/{permissionId}` - Remover permiso de rol

## Ejemplo Práctico: Agregar Permisos de Productos

### 1. Crear Permisos Base

```bash
# Crear permiso para productos
curl -X POST "http://localhost:5001/api/permissions" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "Product",
    "description": "Permisos para gestión de productos"
  }'

# Crear permiso para categorías
curl -X POST "http://localhost:5001/api/permissions" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "key": "ProductCategory",
    "description": "Permisos para gestión de categorías de productos"
  }'
```

### 2. Obtener IDs de Permisos

```bash
# Obtener todos los permisos
curl -X GET "http://localhost:5001/api/permissions" \
  -H "Authorization: Bearer {token}"
```

### 3. Asignar Permisos a un Rol

```bash
# Asignar permisos al rol "Administrador"
curl -X POST "http://localhost:5001/api/roles/{roleId}/permissions" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "permissionIds": [
      "product-permission-id",
      "category-permission-id"
    ]
  }'
```

## Consideraciones Importantes

### 1. Validaciones

- No se pueden crear permisos con claves duplicadas
- No se pueden eliminar permisos que están asignados a roles
- Los permisos deben existir antes de asignarlos a roles

### 2. Seguridad

- Todos los endpoints requieren autorización
- Se debe usar JWT token válido
- Los permisos se validan en cada operación

### 3. Escalabilidad

- El sistema está preparado para agregar nuevas acciones
- Se puede extender fácilmente con nuevos tipos de permisos
- La estructura permite permisos granulares por módulo

## Mejoras Futuras Sugeridas

1. **Acciones Granulares:** Implementar endpoints para configurar acciones específicas
2. **Permisos por Módulo:** Crear permisos específicos por módulo (CRM, Inventario, etc.)
3. **Permisos Condicionales:** Implementar permisos basados en condiciones
4. **Auditoría:** Agregar logging de cambios en permisos
5. **Plantillas de Roles:** Crear plantillas predefinidas de roles con permisos

## Estructura de Base de Datos

```sql
-- Tabla de permisos
CREATE TABLE "Permissions" (
    "Id" uuid PRIMARY KEY,
    "Key" varchar(50) NOT NULL,
    "Description" varchar(500)
);

-- Tabla de roles
CREATE TABLE "Roles" (
    "Id" uuid PRIMARY KEY,
    "Name" varchar(100) NOT NULL,
    "Description" varchar(500),
    "CompanyId" uuid NOT NULL
);

-- Tabla de relación rol-permiso
CREATE TABLE "RolePermissions" (
    "Id" uuid PRIMARY KEY,
    "RoleId" uuid NOT NULL,
    "PermissionId" uuid NOT NULL,
    "Actions" integer DEFAULT 0
);
```


