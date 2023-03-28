namespace AutoLectureRecorder.WebDriver.UnitTests.WebDriver.Mocks;

public class HttpClientFactoryMock : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return new HttpClient();
    }
}
