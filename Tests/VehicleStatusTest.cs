using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Diagnostics;

public class VehicleStatusTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void SetStatusInUse()
    {
        //Arrange
        var baseUrl = "http://localhost:5135/SetStatusInUse";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        restRequest.AddJsonBody(new {
            Key = "UAV"
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        //queryResult.StatusCode.Should().Be(HttpStatusCode.OK);
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        //queryResult.Content.Should().NotBeNull();
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void Test1()
    {
        Debug.WriteLine("This is Debug.WriteLine");
        Trace.WriteLine("This is Trace.WriteLine");
        Console.WriteLine("This is Console.Writeline");
        TestContext.WriteLine("This is TestContext.WriteLine");
        TestContext.Out.WriteLine("This is TestContext.Out.WriteLine");
        TestContext.Progress.WriteLine("This is TestContext.Progress.WriteLine");
        TestContext.Error.WriteLine("This is TestContext.Error.WriteLine");
        Assert.Pass();
    }

    [Test]
    public void SetStatusStandby()
    {
        //Arrange
        var baseUrl = "http://localhost:5135/SetStatusStandby";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        restRequest.AddJsonBody(new {
            Key = "UAV"
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void EmergencyStop()
    {
        //Arrange
        var baseUrl = "http://localhost:5135/EmergencyStop";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        restRequest.AddJsonBody(new {
            Key = "UAV"
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }
}