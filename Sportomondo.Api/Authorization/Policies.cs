namespace Sportomondo.Api.Authorization
{
    public static class Policies
    {
        public const string ActivityGetAll = "Activity_GetAll";
        public const string ActivityGet = "Activity_Get";
        public const string ActivityCreate = "Activity_Create";
        public const string ActivityDelete = "Activity_Delete";
        public const string ActivityUpdate = "Activity_Update";
        public const string UserChangePassword = "User_ChangePassword";
        public const string UserGetAll = "User_GetAll";
        public const string UserDelete = "User_Delete";
        public const string UserChangeRole = "User_ChangeRole";
        public const string AchievementGetAll = "Achievement_GetAll";
        public const string AchievementCreate = "Achievement_Create";
        public const string AchievementDelete = "Achievement_Delete";
        public const string AchievementCheck = "Achievement_Check";
    }
}
