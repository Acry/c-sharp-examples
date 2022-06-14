using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace c_sharp_examples;

public class ForEachAsync
{
	private readonly string[] _userHandlers =
	{
		"users/okyrylchuk",
		"users/shanselman",
		"users/jaredpar",
		"users/davidfowl"
	};

	[Test]
	public async Task DotNet6()
	{
		using HttpClient client = new()
		{
			BaseAddress = new Uri("https://api.github.com")
		};
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotNet", "6"));

		ParallelOptions parallelOptions = new()
		{
			MaxDegreeOfParallelism = 3
		};

		await Parallel.ForEachAsync(_userHandlers, parallelOptions, async (uri, token) =>
		{
			var user = await client.GetFromJsonAsync<GitHubUser>(uri, token);
			TestContext.WriteLine($"Name: {user.Name}\nBio: {user.Bio}\n");
		});
	}

	[Test]
	public async Task DotNet5()
	{
		using HttpClient client = new()
		{
			BaseAddress = new Uri("https://api.github.com")
		};
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DotNet", "5"));

		ParallelOptions parallelOptions = new()
		{
			MaxDegreeOfParallelism = 3
		};

		// await Task.Run(() => Parallel.ForEach(_userHandlers, parallelOptions, async (uri, token) =>
		// {
		// 	var user = await client.GetFromJsonAsync<GitHubUser>(uri, token);
		// 	TestContext.WriteLine($"Name: {user.Name}\nBio: {user.Bio}\n");
		// }));
	}

	/// <summary>
	///     Prove a point
	///     <inheritdoc cref="BillingsService.GetDiagnosesOfBilling" />
	/// </summary>
	[Test]
	public void MyInterest0()
	{
		// Taken from GetDiagnosesOfBilling
		// Get ou links
		// var bsnrList = hevosResult.Select(dia => dia.Bsnr).Distinct();
		// var practiceLinks = new ConcurrentBag<OrganizationUnitLink>();
		// var practiceTask = Parallel.ForEachAsync(bsnrList, (x, _) =>
		// {
		// 	var practice =
		// 		_organizationUnitCacheResolvingService.ResolveByBsnr(x);
		//
		// 	practiceLinks.Add(new OrganizationUnitLink(practice.Result.Id, practice.Result.Name, practice.Result.Bsnr));
		//
		// 	return ValueTask.CompletedTask;
		// });
		//
		// Task.WaitAll(patientTask, practiceTask);


		// Synthetischer Nachbau:

		// get 10 random integer between 50 and 250
		var integers = new List<int>();
		for (var i = 0; i < 10; i++) integers.Add(GetRandomInteger());
		IEnumerable<int> ints = integers;
		ints.Count().Should().Be(10);

		var expected = ints.Sum() * 2 + ints.Sum() * 3;

		var stopWatch = new Stopwatch();
        stopWatch.Start();
		var ersteFolge = new ConcurrentBag<int>();

		var ersterTask = Parallel.ForEachAsync(ints, (x, _) =>
		{
			var zahl = DoubleInteger(x);
			ersteFolge.Add(zahl);
			return ValueTask.CompletedTask;
		});

		var zweiteFolge = new ConcurrentBag<int>();

		var zweiterTask = Parallel.ForEachAsync(ints, (x, _) =>
		{
			var zahl = TripleInteger(x);
			zweiteFolge.Add(zahl);
			return ValueTask.CompletedTask;
		});

		Task.WaitAll(ersterTask, zweiterTask);
		stopWatch.Stop();
		TestContext.WriteLine(stopWatch.ElapsedMilliseconds);
		var result = ersteFolge.Sum() + zweiteFolge.Sum();
		result.Should().Be(expected);
	}

	[Test]
	public void MyInterest1()
	{
		var integers = new List<int>();
		for (var i = 0; i < 10; i++) integers.Add(GetRandomInteger());
		IEnumerable<int> ints = integers;
		ints.Count().Should().Be(10);

		var expected = ints.Sum() * 2 + ints.Sum() * 3;
		var stopWatch = new Stopwatch();
        stopWatch.Start();
		var ersteFolge = new ConcurrentBag<int>();

		Parallel.ForEach(ints, (x, _) =>
		{
			var zahl = DoubleInteger(x);
			ersteFolge.Add(zahl);
		});

		var zweiteFolge = new ConcurrentBag<int>();

		Parallel.ForEach(ints, (x, _) =>
		{
			var zahl = TripleInteger(x);
			zweiteFolge.Add(zahl);
		});
		stopWatch.Stop();
		TestContext.WriteLine(stopWatch.ElapsedMilliseconds);
		var result = ersteFolge.Sum() + zweiteFolge.Sum();
		result.Should().Be(expected);
	}

	[Test]
	public async Task MyInterest2()
	{
		var integers = new List<int>();
		for (var i = 0; i < 10; i++) integers.Add(GetRandomInteger());
		// Creates the same integers, dunno if this is defined behaviour:
		// IEnumerable<int> listOfMyObjetcs = Enumerable.Repeat(GetRandomInteger(), 10);
		IEnumerable<int> ints = integers;
		ints.Count().Should().Be(10);

		var expected = ints.Sum() * 2 + ints.Sum() * 3;

		var stopWatch = new Stopwatch();
        stopWatch.Start();
		var ersteFolge = new ConcurrentBag<int>();

		var ersterTask = Parallel.ForEachAsync(ints, async (x, _) =>
		{
			var zahl = await DoubleIntegerAsync(x);
			ersteFolge.Add(zahl);
		});

		var zweiteFolge = new ConcurrentBag<int>();

		var zweiterTask = Parallel.ForEachAsync(ints, async (x, _) =>
		{
			var zahl = await TripleIntegerAsync(x);
			zweiteFolge.Add(zahl);
		});

		await Task.WhenAll(ersterTask, zweiterTask);
		stopWatch.Stop();
		TestContext.WriteLine(stopWatch.ElapsedMilliseconds);

		var result = ersteFolge.Sum() + zweiteFolge.Sum();
		result.Should().Be(expected);
	}

		[Test]
	public async Task MyInterest3()
	{
		var integers = new List<int>();
		for (var i = 0; i < 10; i++) integers.Add(GetRandomInteger());
		// Creates the same integers, dunno if this is defined behaviour:
		// IEnumerable<int> listOfMyObjetcs = Enumerable.Repeat(GetRandomInteger(), 10);
		IEnumerable<int> ints = integers;
		ints.Count().Should().Be(10);

		var expected = ints.Sum() * 2 + ints.Sum() * 3;

		var stopWatch = new Stopwatch();
        stopWatch.Start();
		var ersteFolge = new ConcurrentBag<int>();
		var zweiteFolge = new ConcurrentBag<int>();
		
		var task = Parallel.ForEachAsync(ints, async (x, _) =>
		{
			ersteFolge.Add(await DoubleIntegerAsync(x));
			zweiteFolge.Add(await TripleIntegerAsync(x));
		});

		await task;
		stopWatch.Stop();
		TestContext.WriteLine(stopWatch.ElapsedMilliseconds);

		var result = ersteFolge.Sum() + zweiteFolge.Sum();
		result.Should().Be(expected);
	}

	private async Task WaitNMillisecondsAsync(int milliseconds)
	{
		await Task.Delay(milliseconds);
	}

	private async Task<int> GetRandomIntegerAsync()
	{
		var randomInteger = await Task.FromResult(GetRandomInteger());
		return randomInteger;
	}

	private static int GetRandomInteger()
	{
		var myObject = new Random(Guid.NewGuid().GetHashCode());
		var randomInteger = myObject.Next(50, 250);
		return randomInteger;
	}

	private Task<int> DoubleIntegerAsync(int zahl)
	{
		return Task.FromResult(zahl*2);
	}


	private Task<int> TripleIntegerAsync(int zahl)
	{
		return Task.FromResult(zahl*3);
	}
		
	private static int DoubleInteger(int zahl)
	{
		return zahl * 2;
	}

	private static int TripleInteger(int zahl)
	{
		return zahl * 3;
	}

	private class GitHubUser
	{
		public string Name { get; set; }
		public string Bio { get; set; }
	}
}