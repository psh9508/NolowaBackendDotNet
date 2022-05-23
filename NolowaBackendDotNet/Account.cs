using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet
{
    public partial class Account
    {
        public Account()
        {
            FollowerDestinationAccounts = new HashSet<Follower>();
            FollowerSourceAccounts = new HashSet<Follower>();
            Posts = new HashSet<Post>();
            ProfileInfos = new HashSet<ProfileInfo>();
            SearchHistories = new HashSet<SearchHistory>();
        }

        public long Id { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long? ProfileImageId { get; set; }
        public DateTime InsertDate { get; set; }

        public virtual ProfileImage ProfileImage { get; set; }
        public virtual ICollection<Follower> FollowerDestinationAccounts { get; set; }
        public virtual ICollection<Follower> FollowerSourceAccounts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<ProfileInfo> ProfileInfos { get; set; }
        public virtual ICollection<SearchHistory> SearchHistories { get; set; }
    }
}
