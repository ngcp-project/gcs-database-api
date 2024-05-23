
using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using Database.Models;
namespace DatabaseAPI.Tests;
public class MissionStageTest{
    [Test]
    public void PostMissionStage(){
        //Arrange
        var baseUrl = "http://localhost:5135/MissionStage";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        //Arrange Post Body
        VehicleData coord1 = new VehicleData();          
        VehicleData coord2 = new VehicleData();          
        string coor1 = coord1.ToString();                
        string coor2 = coord2.ToString();                
        var twoDimensionalArray = new string[] {coord1, coord2};
        restRequest.AddJsonBody(new {
            missionName = "Polygon",
            stageName = "Moonwalk",
            vehicleKeys = twoDimensionalArray,
        });
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void GetMissionStage(){
        //Arrange
        var baseUrl = "http://localhost:5135/MissionStage";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }

    [Test]
    public void DeleteMissionStage(){
        //Arrange
        var baseUrl = "http://localhost:5135/MissionStage";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Delete);
        //Act
        var queryResult = client.Execute(restRequest);
        //Assert
        Assert.That(queryResult.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status code is not OK");
        Assert.That(queryResult.Content, Is.Not.Null, "Response content is null");
    }
}