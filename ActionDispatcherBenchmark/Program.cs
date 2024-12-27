using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using ClassLibrary;

namespace ActionDispatcherBenchmark
{
    [SimpleJob(RunStrategy.ColdStart, iterationCount: 5)]
    public class GeneratedVsReflection
    {
        private const int iterationCount = 1_000_000_000;
        private readonly string[] actionNames;
        private readonly Random random;

        private MyGeneratedActionDispatcher? generatedDispatcher;
        private MyReflectionActionDispatcher? reflectionDispatcher;

        public GeneratedVsReflection()
        {
            random = new Random(42);

            actionNames = typeof(MyClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(MyActionAttribute), false).Length != 0)
                .Select(m=> m.Name)
                .ToArray();

        }

        [Benchmark]
        public void Generated()
        {
            generatedDispatcher = new MyGeneratedActionDispatcher();
            for (int i = 0; i < iterationCount; i++)
            {
                string name = actionNames[random.Next(0, actionNames.Length)];
                generatedDispatcher.Dispatch(name);
            }

        }

        [Benchmark]
        public void Reflection()
        {
            reflectionDispatcher = new MyReflectionActionDispatcher();
            for (int i = 0; i < iterationCount; i++)
            {
                string name = actionNames[random.Next(0, actionNames.Length)];
                reflectionDispatcher.Dispatch(name);
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            _ = BenchmarkRunner.Run<GeneratedVsReflection>();
        }
    }
}
