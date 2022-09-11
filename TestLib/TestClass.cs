namespace TestLib
{
    public partial class TestClass
    {
        // Does not work.
        [BackedProperty]
        public int MyProp
        {
            get => _MyProp;
            set => _MyProp = value;
        }
    }
}
