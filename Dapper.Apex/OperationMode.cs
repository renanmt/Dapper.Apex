
namespace Dapper.Apex
{
    /// <summary>
    /// Defines the way the operation will be sent to the database for multiple entities.
    /// </summary>
    public enum OperationMode
    {
        OneByOne = 0,
        SingleShot = 1
    }
}
