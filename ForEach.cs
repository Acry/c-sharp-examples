using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace c_sharp_examples;

[TestFixture]
public class ForEach
{
	/// <summary>
	///     sequential ForEach vs LINQ-Expression vs Parallel.ForEach
	/// </summary>
	[Test]
	public void Case1()
	{
		const int limit = 2_000_000;
		var numbers = Enumerable.Range(0, limit).ToList();

		// LINQ-Expression
		var watchLinq = Stopwatch.StartNew();
		var linqResult = numbers.Where(IsPrime).ToList();
		watchLinq.Stop();

		// Sequential ForEach
		var watch = Stopwatch.StartNew();
		var forEachResult = new List<int>();
		foreach (var number in numbers)
			if (IsPrime(number))
				forEachResult.Add(number);
		watch.Stop();

		// Parallel.ForEach
		var watchForParallel = Stopwatch.StartNew();
		var parallelResult = new ConcurrentBag<int>();

		Parallel.ForEach(numbers, number =>
		{
			if (IsPrime(number)) parallelResult.Add(number);
		});
		watchForParallel.Stop();

		// Output
		TestContext.WriteLine(
			$"Classical foreach loop | Total primes : {forEachResult.Count} | Time Taken : {watch.ElapsedMilliseconds} ms.");
		TestContext.WriteLine(
			$"LINQ-Expression | Total primes : {linqResult.Count} | Time Taken : {watchLinq.ElapsedMilliseconds} ms.");
		TestContext.WriteLine(
			$"Parallel.ForEach loop  | Total primes : {parallelResult.Count} | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");
	}

	/// <summary>
	///     IsPrime returns true if number is Prime, else false.(https://en.wikipedia.org/wiki/Prime_number)
	/// </summary>
	/// <param name="number"></param>
	/// <returns></returns>
	private static bool IsPrime(int number)
	{
		if (number < 2) return false;

		for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
			if (number % divisor == 0)
				return false;
		return true;
	}
}