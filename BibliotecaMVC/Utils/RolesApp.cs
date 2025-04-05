namespace BibliotecaMVC.Utils
{
    public static class RolesApp
    {
        public const string Admin = "Administrador";
        public const string Lector = "Lector";
        public const string Bibliotecario = "Bibliotecario";

        public static readonly string[] Roles = new string[] { Admin, Lector, Bibliotecario };
    }
}