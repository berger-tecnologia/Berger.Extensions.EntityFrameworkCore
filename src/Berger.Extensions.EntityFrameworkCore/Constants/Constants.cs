namespace Berger.Extensions.EntityFrameworkCore
{
    public static class BaseColumns
    {
        public const string IsDeleted = "IS_DLT";
        public const string DeletedOn = "DLT_ON";
        public const string CreatedOn = "CRT_ON";
        public const string ModifiedOn = "MDF_ON";
    }
    public static class KeyColumns
    {
        public const string Id = "ID";
    }
    public static class UserColumns
    {
        public const string UserId = "USER_ID";
        public const string ReceiverId = "RECEIVER_ID";
    }
}