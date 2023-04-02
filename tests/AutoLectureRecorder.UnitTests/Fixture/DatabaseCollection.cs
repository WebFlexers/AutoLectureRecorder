namespace AutoLectureRecorder.UnitTests.Fixture;

[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DbInitializerFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
