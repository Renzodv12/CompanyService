# Documentación de Endpoints - Sistema de Gestión de Permisos

## Endpoints de Roles

### GET /api/roles
Obtiene todos los roles del sistema.
- **Autorización**: Requerida
- **Respuesta**: Lista de roles con sus permisos

### GET /api/roles/{id}
Obtiene un rol específico por ID.
- **Parámetros**: `id` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: Datos del rol con sus permisos

### POST /api/roles
Crea un nuevo rol.
- **Body**: 
```json
{
  "name": "string",
  "description": "string",
  "companyId": "guid"
}
```
- **Autorización**: Requerida
- **Respuesta**: Rol creado

### PUT /api/roles/{id}
Actualiza un rol existente.
- **Parámetros**: `id` (Guid) - ID del rol
- **Body**: 
```json
{
  "name": "string",
  "description": "string",
  "companyId": "guid"
}
```
- **Autorización**: Requerida
- **Respuesta**: Rol actualizado

### DELETE /api/roles/{id}
Elimina un rol.
- **Parámetros**: `id` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: 204 No Content

### GET /api/roles/{id}/permissions
Obtiene los permisos de un rol específico.
- **Parámetros**: `id` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: Lista de permisos del rol

### POST /api/roles/{roleId}/permissions/{permissionId}
Asigna un permiso a un rol.
- **Parámetros**: 
  - `roleId` (Guid) - ID del rol
  - `permissionId` (Guid) - ID del permiso
- **Autorización**: Requerida
- **Respuesta**: Confirmación de asignación

### DELETE /api/roles/{roleId}/permissions/{permissionId}
Remueve un permiso de un rol.
- **Parámetros**: 
  - `roleId` (Guid) - ID del rol
  - `permissionId` (Guid) - ID del permiso
- **Autorización**: Requerida
- **Respuesta**: 204 No Content

### GET /api/roles/{id}/users
Obtiene los usuarios asignados a un rol.
- **Parámetros**: `id` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: Lista de usuarios del rol

## Endpoints de Permisos

### GET /api/permissions
Obtiene todos los permisos del sistema.
- **Autorización**: Requerida
- **Respuesta**: Lista de permisos

### GET /api/permissions/{id}
Obtiene un permiso específico por ID.
- **Parámetros**: `id` (Guid) - ID del permiso
- **Autorización**: Requerida
- **Respuesta**: Datos del permiso

### POST /api/permissions
Crea un nuevo permiso.
- **Body**: 
```json
{
  "key": "string",
  "description": "string"
}
```
- **Autorización**: Requerida
- **Respuesta**: Permiso creado

### PUT /api/permissions/{id}
Actualiza un permiso existente.
- **Parámetros**: `id` (Guid) - ID del permiso
- **Body**: 
```json
{
  "key": "string",
  "description": "string"
}
```
- **Autorización**: Requerida
- **Respuesta**: Permiso actualizado

### DELETE /api/permissions/{id}
Elimina un permiso.
- **Parámetros**: `id` (Guid) - ID del permiso
- **Autorización**: Requerida
- **Respuesta**: 204 No Content

### GET /api/permissions/{id}/roles
Obtiene los roles que tienen asignado un permiso específico.
- **Parámetros**: `id` (Guid) - ID del permiso
- **Autorización**: Requerida
- **Respuesta**: Lista de roles con el permiso

## Endpoints de Usuarios

### GET /api/users
Obtiene todos los usuarios del sistema.
- **Autorización**: Requerida
- **Respuesta**: Lista de usuarios

### GET /api/users/{id}
Obtiene un usuario específico por ID.
- **Parámetros**: `id` (Guid) - ID del usuario
- **Autorización**: Requerida
- **Respuesta**: Datos del usuario

### POST /api/users
Crea un nuevo usuario.
- **Body**: 
```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "password": "string",
  "ci": "string",
  "birthDate": "2024-01-01T00:00:00Z",
  "typeAuth": "string"
}
```
- **Autorización**: Requerida
- **Respuesta**: Usuario creado

### PUT /api/users/{id}
Actualiza un usuario existente.
- **Parámetros**: `id` (Guid) - ID del usuario
- **Body**: 
```json
{
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "ci": "string",
  "birthDate": "2024-01-01T00:00:00Z",
  "typeAuth": "string"
}
```
- **Autorización**: Requerida
- **Respuesta**: Usuario actualizado

### DELETE /api/users/{id}
Elimina un usuario.
- **Parámetros**: `id` (Guid) - ID del usuario
- **Autorización**: Requerida
- **Respuesta**: 204 No Content

### GET /api/users/{id}/roles
Obtiene los roles asignados a un usuario.
- **Parámetros**: `id` (Guid) - ID del usuario
- **Autorización**: Requerida
- **Respuesta**: Lista de roles del usuario

### POST /api/users/{userId}/roles/{roleId}
Asigna un rol a un usuario.
- **Parámetros**: 
  - `userId` (Guid) - ID del usuario
  - `roleId` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: Confirmación de asignación

### DELETE /api/users/{userId}/roles/{roleId}
Remueve un rol de un usuario.
- **Parámetros**: 
  - `userId` (Guid) - ID del usuario
  - `roleId` (Guid) - ID del rol
- **Autorización**: Requerida
- **Respuesta**: 204 No Content

### PUT /api/users/{id}/password
Cambia la contraseña de un usuario.
- **Parámetros**: `id` (Guid) - ID del usuario
- **Body**: 
```json
{
  "currentPassword": "string",
  "newPassword": "string"
}
```
- **Autorización**: Requerida
- **Respuesta**: Confirmación de cambio

## Configuración

Para usar estos endpoints, asegúrate de:

1. **Registrar los endpoints** en tu `Program.cs`:
```csharp
using CompanyService.WebApi.Extensions;

// ... otras configuraciones

app.MapAllEndpoints();
```

2. **Configurar autenticación y autorización** en tu aplicación.

3. **Asegurar que las entidades** `User`, `Role`, `Permission`, `RolePermission` y `UserCompany` estén correctamente configuradas en tu DbContext.

4. **Validar que el enum `TypeAuth`** esté definido en tu proyecto.

## Notas de Seguridad

- Todos los endpoints requieren autorización
- Las contraseñas se almacenan con hash y salt
- Se valida la unicidad de emails y claves de permisos
- Se previene la eliminación de entidades con dependencias

## Códigos de Estado HTTP

- **200 OK**: Operación exitosa
- **201 Created**: Recurso creado exitosamente
- **204 No Content**: Eliminación exitosa
- **400 Bad Request**: Error en la solicitud o validación
- **404 Not Found**: Recurso no encontrado
- **401 Unauthorized**: No autorizado