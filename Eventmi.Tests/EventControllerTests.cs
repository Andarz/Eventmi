using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventmi.Core.Models.Event;
using Eventmi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using RestSharp;

namespace Eventmi.Tests
{
	[TestFixture]
	public class EventControllerTests
	{
		private RestClient _client;
		private const string baseURL = "https://localhost:7236";

		[SetUp]
		public void Setup()
		{
			_client = new RestClient(baseURL);
		}

		[Test]
		public void GetAllEvents_ReturnsSuccessStatusCode()
		{
			// Arrange
			var request = new RestRequest("/Event/All", Method.Get);

			// Act
			var response = _client.Execute(request);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
		}

		[Test]
		public void Add_PostRequest_AddsEventAndRedirects()
		{
			// Arrange
			var addedEvent = new EventFormModel()
			{
				Name = "New added event from RestSharp",
				Start = new DateTime(2024, 03, 13, 11, 0, 0),
				End = new DateTime(2024, 03, 14, 10, 0, 0),
				Place = "Riga"
			};

			var request = new RestRequest("/Event/Add", Method.Post);

			request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

			request.AddParameter("name", addedEvent.Name);
			request.AddParameter("start", addedEvent.Start.ToString("MM/dd/yyyy hh:mm tt"));
			request.AddParameter("end", addedEvent.End.ToString("MM/dd/yyyy hh:mm tt"));
			request.AddParameter("place", addedEvent.Place);

			// Act
			var response = _client.Execute(request);

			// Assert
			Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
			Assert.IsTrue(CheckEventExists(addedEvent.Name));
		}

		private bool CheckEventExists(string eventName)
		{
			var options = new DbContextOptionsBuilder<EventmiContext>()
				.UseSqlServer("Server=PC05;Database=Eventmi;Trusted_Connection=True;MultipleActiveResultSets=true")
				.Options;

			using (var context = new EventmiContext(options))
			{
				return context.Events.Any(e => e.Name == eventName);
			}
		}
	}
}
