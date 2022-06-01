using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class Account
    {
        public Account()
        {
            FollowerDestinationAccounts = new HashSet<Follower>();
            FollowerSourceAccounts = new HashSet<Follower>();
            Posts = new HashSet<Post>();
            SearchHistories = new HashSet<SearchHistory>();
        }

        public long Id { get; set; }
        public string UserId { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long? ProfileInfoId { get; set; }
        public DateTime InsertDate { get; set; }

        public virtual ProfileInfo ProfileInfo { get; set; }
        public virtual ICollection<Follower> FollowerDestinationAccounts { get; set; }
        public virtual ICollection<Follower> FollowerSourceAccounts { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<SearchHistory> SearchHistories { get; set; }
    }
}
