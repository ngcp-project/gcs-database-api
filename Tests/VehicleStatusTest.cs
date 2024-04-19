using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
//using Database.Models;

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
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
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
/*
    [Test]
    public void SetStatusStandby()
    {
        //Arrange
        var baseUrl = "http://localhost:5135/SetStatusStandby";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        restRequest.AddJsonBody(new {
            Key = "UAV"
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        queryResult.StatusCode.Should().Be(HttpStatusCode.OK);
        queryResult.Content.Should().NotBeNull();
        //Assert.Pass();
    }
*/
    [Test]
    public void SearchBooks()
    {
        var baseUrl = "https://fakerestapi.azurewebsites.net/api/v1/Books";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        RestResponse restResponse = client.Execute(restRequest);
        restResponse.Should().NotBeNull();
        restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}