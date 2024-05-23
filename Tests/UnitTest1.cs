using System.Net;
using FluentAssertions;
using RestSharp;
namespace DatabaseAPI.Tests;
public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestingInitalization()
    {

        Assert.Pass();
    }

    [Test]
    public void TestRESTEndpointCall()
    {
        var baseUrl = "https://fakerestapi.azurewebsites.net/api/v1/Books";
        RestClient client = new RestClient(baseUrl);
        RestRequest restRequest = new RestRequest(baseUrl, Method.Get);
        RestResponse restResponse = client.Execute(restRequest);
        restResponse.Should().NotBeNull();
        restResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

}