using NolowaNetwork.Models.Message;

namespace SharedLib.Models
{
    public class User : NetMessageBase
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        //public List<Follower> Follower { get; set; }
        //public ProfileInfo ProfileInfo { get; set; } = new();
        public string Jwt { get; set; } = string.Empty;
        public string ProfileImageFile { get; set; } = string.Empty;
    }
}

//message LoginRes
//{
//    int64 id = 1;
//    string user_id = 2;
//    string account_name = 3;
//    google.protobuf.Timestamp join_date = 4;
//    string password = 5;
//    string email = 6;
//    repeated follower followers = 7;
//    profile_info profile_info = 8;
//    string jwt_token = 9;
//    string profile_image_file = 10;
//}