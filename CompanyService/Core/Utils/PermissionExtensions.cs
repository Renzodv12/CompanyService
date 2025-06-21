using CompanyService.Core.Enums;

namespace CompanyService.Core.Utils
{
    public static class PermissionExtensions
    {
        public static bool HasPermission(this int actions, PermissionAction permission)
        {
            return ((PermissionAction)actions).HasFlag(permission);
        }

        public static bool HasAllPermissions(this int actions, PermissionAction permissions)
        {
            return (actions & (int)permissions) == (int)permissions;
        }

        public static List<PermissionAction> GetPermissions(this int value)
        {
            var permissions = new List<PermissionAction>();

            if (HasPermission(value, PermissionAction.View)) permissions.Add(PermissionAction.View);
            if (HasPermission(value, PermissionAction.Create)) permissions.Add(PermissionAction.Create);
            if (HasPermission(value, PermissionAction.Edit)) permissions.Add(PermissionAction.Edit);
            if (HasPermission(value, PermissionAction.Delete)) permissions.Add(PermissionAction.Delete);

            return permissions;
        }

        public static List<string> GetPermissionsNames(this int value)
        {
            var permissions = new List<string>();

            if (HasPermission(value, PermissionAction.View)) permissions.Add(PermissionAction.View.ToString());
            if (HasPermission(value, PermissionAction.Create)) permissions.Add(PermissionAction.Create.ToString());
            if (HasPermission(value, PermissionAction.Edit)) permissions.Add(PermissionAction.Edit.ToString());
            if (HasPermission(value, PermissionAction.Delete)) permissions.Add(PermissionAction.Delete.ToString());

            return permissions;
        }
    }
}
