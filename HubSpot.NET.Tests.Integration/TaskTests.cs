using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Task;
using HubSpot.NET.Api.Task.Dto;
using HubSpot.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class TaskTests
	{
		[TestMethod]
		public void Create_SampleDetails_IdProeprtyIsSet()
		{
			// Arrange
			var taskApi = new HubSpotTaskApi(TestSetUp.Client);
			var sampleTask = new TaskHubSpotModel
			{
				Subject = "New Created Task",
				Notes = "Some created notes",
				DueDate = DateTime.UtcNow.Date.AddDays(1)
			};

			// Act
			TaskHubSpotModel task = taskApi.Create(sampleTask);

			try
			{
				// Assert
				Assert.IsNotNull(task.Id, "The Id was not set and returned.");
				Assert.AreEqual(sampleTask.Subject, task.Subject);
				Assert.AreEqual(sampleTask.Notes, task.Notes);
				Assert.AreEqual(sampleTask.DueDate, task.DueDate);
			}
			finally
			{
				// Clean-up
				taskApi.Delete(task.Id.Value);
			}
		}

		[TestMethod]
		public void Update_SampleDetails_PropertiesAreUpdated()
		{
			// Arrange
			var taskApi = new HubSpotTaskApi(TestSetUp.Client);
			var sampleTask = new TaskHubSpotModel
			{
				Subject = "New Updated Task",
				Notes = "Some updated notes",
				DueDate = DateTime.UtcNow.Date.AddDays(1)
			};

			TaskHubSpotModel task = taskApi.Create(sampleTask);

			task.Subject = "New Further Updated Task";
			task.Notes = "Some Further updated notes";
			task.DueDate = DateTime.UtcNow.Date.AddDays(2);

			// Act
			taskApi.Update(task);

			try
			{
				// Assert
				Assert.AreNotEqual(sampleTask.Subject, task.Subject);
				Assert.AreNotEqual(sampleTask.Notes, task.Notes);
				Assert.AreNotEqual(sampleTask.DueDate, task.DueDate);
				Assert.AreEqual("New Further Updated Task", task.Subject);
				Assert.AreEqual("Some Further updated notes", task.Notes);
				Assert.AreEqual(DateTime.UtcNow.Date.AddDays(2), task.DueDate);

				// Second Act
				task = taskApi.GetById<TaskHubSpotModel>(task.Id.Value);

				// Second Assert
				Assert.AreNotEqual(sampleTask.Subject, task.Subject);
				Assert.AreNotEqual(sampleTask.Notes, task.Notes);
				Assert.AreNotEqual(sampleTask.DueDate, task.DueDate);
				Assert.AreEqual("New Further Updated Task", task.Subject);
				Assert.AreEqual("Some Further updated notes", task.Notes);
				Assert.AreEqual(DateTime.UtcNow.Date.AddDays(2), task.DueDate);
			}
			finally
			{
				// Clean-up
				taskApi.Delete(task.Id.Value);
			}
		}

		[TestMethod]
		public void Delete_sampleTask_CompanyIsDeleted()
		{
			// Arrange
			var taskApi = new HubSpotTaskApi(TestSetUp.Client);
			var sampleTask = new TaskHubSpotModel
			{
				Subject = "New Deleted Task",
				Notes = "Some Deleted notes",
				DueDate = DateTime.UtcNow.Date.AddDays(3)
			};

			TaskHubSpotModel company = taskApi.Create(sampleTask);

			// Act
			taskApi.Delete(company.Id.Value);

			// Assert
			company = taskApi.GetById<TaskHubSpotModel>(company.Id.Value);
			Assert.IsNull(company, "The task was searchable and not deleted.");
		}
	}
}