using Xunit;

namespace MindCareAi.Tests.Testing;

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
