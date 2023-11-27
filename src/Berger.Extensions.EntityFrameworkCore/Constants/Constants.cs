namespace Berger.Extensions.EntityFrameworkCore
{
    public static class BaseColumns
    {
        public const string IsDeleted = "IS_DELETED";
        public const string DeletedOn = "DELETED_ON";
        public const string CreatedOn = "CREATED_ON";
        public const string ModifiedOn = "MODIFIED_ON";
    }
    public static class KeyColumns
    {
        public const string Id = "ID";
    }
    public static class UserColumns
    {
        public const string UserId = "USER_ID";
        public const string OwnerId = "OWNER_ID";
        public const string ReceiverId = "RECEIVER_ID";
    }
}