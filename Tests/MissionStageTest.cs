
using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Diagnostics;
using Database.Models;
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
        string coor1 = coord1.ToString();                //wrong
        string coor2 = coord2.ToString();                //wrong
        var twoDimensionalArray = new Coordinate[] {coord1, coord2};    //wrong
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

    }

    [Test]
    public void DeleteMissionStage(){

    }
}