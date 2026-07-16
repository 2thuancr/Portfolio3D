namespace Portfolio3D.Permissions;

public static class Portfolio3DPermissions
{
    public const string GroupName = "Portfolio3D";

    public static class Projects
    {
        public const string Default = GroupName + ".Projects";

        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Publish = Default + ".Publish";
        public const string Reorder = Default + ".Reorder";
    }
}
