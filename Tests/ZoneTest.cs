using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using Database.Models;

public class ZoneTest {
    [Test]
    public void getInZones() {
        //Arrange
        var baseUrl = "http://localhost:5135/SetStatusInUse";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void getOutZones() {
        //Arrange
        var baseUrl = "http://localhost:5135/SetStatusInUse";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void postKeepIn(){
        var baseUrl = "http://localhost:5135/EmergencyStop";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        Coordinate coord1 = new Coordinate(1.0, 2.0);
        Coordinate coord2 = new Coordinate(1.5, 2.69);
        var coordArray = new Coordinate[] {coord1, coord2};
        restRequest.AddJsonBody(new {
            name = "Mexico",
            shapeType = "Polygon",
            coordinates = coordArray
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }
}