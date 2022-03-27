using System;
using System.Collections.Generic;

#nullable disable

namespace NolowaBackendDotNet.Models
{
    public partial class Account
    {
        public Account()
        {
            Followers = new HashSet<Follower>();
            Posts = new HashSet<Post>();
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
        public virtual ICollection<Follower> Followers { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<SearchHistory> SearchHistories { get; set; }
    }
}
