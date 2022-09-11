
namespace TestLib
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal class BackedPropertyAttribute : System.Attribute {
        public BackedPropertyAttribute() : base() { }
    }
}