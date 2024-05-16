
using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using Database.Models;
using System.Text.Json;
public class MissionStageTest{
    [Test]
    public void PostMissionStage(){
        //Arrange
        var baseUrl = "http://localhost:5135/MissionStage";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Post);
        //Arrange Post Body
        Coordinate coord1 = new Coordinate(1.0, 2.0);                   //wrong
        Coordinate coord2 = new Coordinate(1.5, 2.69);                  //wrong
        string coor1 = JsonSerializer.Serialize(coord1);                //wrong
        string coor2 = JsonSerializer.Serialize(coord2);                //wrong
        var twoDimensionalArray = new Coordinate[] {coord1, coord2};    //wrong
        restRequest.AddJsonBody(new {
            key = "Mexico",
            stageID = "Polygon",
            stageName = "Moonwalk"
            stageStatus = "NOT_STARTED"
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

    }

    [Test]
    public void DeleteMissionStage(){

    }
}