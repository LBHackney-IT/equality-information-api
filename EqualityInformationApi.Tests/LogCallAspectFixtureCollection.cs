using Hackney.Core.Testing.Shared;
using Xunit;

namespace EqualityInformationApi.Tests
{
    [CollectionDefinition("LogCall collection")]
    public class LogCallAspectFixtureCollection : ICollectionFixture<LogCallAspectFixture>
    { }
}
