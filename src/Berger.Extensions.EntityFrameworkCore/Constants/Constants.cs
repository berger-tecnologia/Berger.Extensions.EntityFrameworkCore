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
        public const string ID = "ID";
    }
    public static class UserColumns
    {
        public const string UserID = "USER_ID";
        public const string OwnerID = "OWNER_ID";
        public const string ReceiverID = "RECEIVER_ID";
    }
}