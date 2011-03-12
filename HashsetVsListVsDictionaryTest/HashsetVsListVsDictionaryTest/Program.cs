using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HashsetVsListVsDictionaryTest
{
    class Program
    {
        private const int NumberOfTestRuns = 5;

        static void Main(string[] args)
        {
            DoTest1();

            DoTest2();

            DoTest3();

            DoTest4();

            DoTest5();

            DoTest6();
        }

        private static void DoTest1()
        {
            var inputs = Enumerable.Range(1, 1000000).ToList();

            DoAddTest(1, "Add 1000000 value types", inputs);
        }

        private static void DoTest2()
        {
            var inputs = Enumerable.Range(1, 1000000)
                            .Select(i => new Person { Name = "TestMe", Age = i })
                            .ToList();

            DoAddTest(2, "Add 1000000 reference types", inputs);
        }

        private static void DoTest3()
        {
            var inputs = Enumerable.Range(1, 10000).ToList();
            
            // look for even numbers
            var targets = inputs.Where(n => n % 2 == 0).ToList();

            DoContainsTest(3, "Run Contains() on half of 10000 value types", inputs, targets);
        }

        private static void DoTest4()
        {
            var inputs = Enumerable.Range(1, 10000)
                            .Select(i => new Person { Name = "TestMe", Age = i }).ToList();

            // look for Person with even age
            var targets = inputs.Where(p => p.Age % 2 == 0).ToList();

            DoContainsTest(4, "Run Contains() on half of 10000 reference types", inputs, targets);
        }

        private static void DoTest5()
        {
            var inputs = Enumerable.Range(1, 10000).ToList();

            // remove even numbers
            var targets = inputs.Where(n => n % 2 == 0).ToList();

            DoRemoveTest(5, "Remove half of 10000 value types", inputs, targets);
        }

        private static void DoTest6()
        {
            var inputs = Enumerable.Range(1, 10000)
                            .Select(i => new Person { Name = "TestMe", Age = i }).ToList();

            // remove Person with even age
            var targets = inputs.Where(p => p.Age % 2 == 0).ToList();

            DoRemoveTest(6, "Remove half of 10000 reference types", inputs, targets);
        }

        /// <summary>
        /// Helper method that performs an Add test using HashSet, List and Dictionary
        /// </summary>
        private static void DoAddTest<T>(int testNumber, string description, List<T> inputs)
        {
            var hashsetResult = PerfTest.DoTest(
                inputs, () => new HashSet<T>(), (hs, i) => hs.Add(i), NumberOfTestRuns);

            var listResult = PerfTest.DoTest(
                inputs, () => new List<T>(), (l, i) => l.Add(i), NumberOfTestRuns);

            var dictResult = PerfTest.DoTest(
                inputs, () => new Dictionary<T, T>(), (dict, i) => dict.Add(i, i), NumberOfTestRuns);

            var dictResult2 = PerfTest.DoTest(
                inputs, () => new Dictionary<T, T>(), (dict, i) => dict[i] = i, NumberOfTestRuns);

            Console.WriteLine(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Add              : {2}
            
List.Add                 : {3}
            
Dictionary.Add           : {4}
            
Dictionary[n] = n        : {5}
------------------------------------------------",
                testNumber,
                description,
                hashsetResult,
                listResult,
                dictResult,
                dictResult2);
        }

        /// <summary>
        /// Helper method that performs a Contains test using HashSet, List and Dictionary
        /// </summary>
        private static void DoContainsTest<T>(
            int testNumber, string description, List<T> inputs, List<T> targets)
        {
            var hashsetResult = PerfTest.DoTest(
                targets, () => new HashSet<T>(inputs), (hs, t) => hs.Contains(t), NumberOfTestRuns);

            var listResult = PerfTest.DoTest(
                targets, () => new List<T>(inputs), (l, t) => l.Contains(t), NumberOfTestRuns);

            var dictResult = PerfTest.DoTest(
                targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.ContainsKey(t), NumberOfTestRuns);

            var dictResult2 = PerfTest.DoTest(
                targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.ContainsValue(t), NumberOfTestRuns);

            Console.WriteLine(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Contains            : {2}
            
List.Contains               : {3}
            
Dictionary.ContainsKey      : {4}
            
Dictionary.ContainsValue    : {5}
------------------------------------------------",
                testNumber,
                description,
                hashsetResult,
                listResult,
                dictResult,
                dictResult2);
        }

        private static void DoRemoveTest<T>(
            int testNumber, string description, List<T> inputs, List<T> targets)
        {
            var hashsetResult = PerfTest.DoTest(
                targets, () => new HashSet<T>(inputs), (hs, t) => hs.Remove(t), NumberOfTestRuns);

            var listResult = PerfTest.DoTest(
                targets, () => new List<T>(inputs), (l, t) => l.Remove(t), NumberOfTestRuns);

            var dictResult = PerfTest.DoTest(
                targets, () => inputs.ToDictionary(i => i, i => i), (dict, t) => dict.Remove(t), NumberOfTestRuns);

            Console.WriteLine(@"
Test {0} ({1}) Result:
------------------------------------------------
HashSet.Remove      : {2}
            
List.Remove         : {3}
            
Dictionary.Remove   : {4}
------------------------------------------------",
                testNumber,
                description,
                hashsetResult,
                listResult,
                dictResult);
        }

        /// <summary>
        /// A static class for executing the performance tests
        /// </summary>
        public static class PerfTest
        {
            public static long DoTest<TCol, TInput>(
                List<TInput> inputs,             // the inputs for the test
                Func<TCol> initCollection,      // initialize a new collection for the test
                Action<TCol, TInput> action,    // the action to perform against the input
                int numberOfRuns)               // how many times do we need to repeat the test?
                where TCol : class, new()
            {
                long totalTime = 0;
                var stopwatch = new Stopwatch();

                for (var i = 0; i < numberOfRuns; i++)
                {
                    // get a new collection for this test run
                    var collection = initCollection();

                    // start the clock and execute the test
                    stopwatch.Start();
                    inputs.ForEach(n => action(collection, n));
                    stopwatch.Stop();

                    // add to the total time
                    totalTime += stopwatch.ElapsedMilliseconds;

                    // reset the stopwatch for the next run
                    stopwatch.Reset();
                }

                var avgTime = totalTime / numberOfRuns;

                return avgTime;
            }
        }

        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
