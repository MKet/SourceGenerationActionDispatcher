// See https://aka.ms/new-console-template for more information

using ClassLibrary;

var generatedDispatcher = new MyGeneratedActionDispatcher();

generatedDispatcher.Dispatch("DoSomething"); // Output: DoSomething executed.
generatedDispatcher.Dispatch("DoSomethingElse"); // Output: DoSomethingElse executed.

var reflectionDispatcher = new MyReflectionActionDispatcher();

reflectionDispatcher.Dispatch("DoSomething"); // Output: DoSomething executed.
reflectionDispatcher.Dispatch("DoSomethingElse"); // Output: DoSomethingElse executed.

Console.ReadKey(); 