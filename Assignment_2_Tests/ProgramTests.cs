using Assignment_2.Program;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assignment_2_Tests.ProgramTests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccess()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/todo");

            response.EnsureSuccessStatusCode();
        }
    }
}
