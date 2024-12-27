// See https://aka.ms/new-console-template for more information

using ClassLibrary;

var generatedDispatcher = new MyGeneratedActionDispatcher();

generatedDispatcher.Dispatch("DoSomething", "Hello", "3"); // Output: Hello was repeated 3 times.
generatedDispatcher.Dispatch("DoSomethingElse", "42.5");      // Output: Value is 42.5

Console.ReadKey(); 