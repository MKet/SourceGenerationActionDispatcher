namespace ClassLibrary
{
    public class MyClass
    {
        public static int Var1 { get; set; }
        public static int Var2 { get; set; }
        public static int Var3 { get; set; }

        [MyAction]
        public static void DoSomething()
        {
            Var1++;
        }

        [MyAction]
        public static void DoSomethingElse()
        {
            Var2++;
        }

        [MyAction]
        public static void DoSomethingElseEntirely()
        {
            Var3++;
        }
    }
}
