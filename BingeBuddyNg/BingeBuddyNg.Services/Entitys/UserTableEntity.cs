using BingeBuddyNg.Services.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.Entitys
{
    public class UserTableEntity : TableEntity
    {
        public const string PartitionKeyValue = "UserEntity";

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public int? Weight { get; set; }
        public string Gender { get; set; }

        public UserTableEntity()
        {

        }

        public UserTableEntity(string id, string displayName, string profileImageUrl, Gender gender, int? weight)
        {
            this.PartitionKey = PartitionKeyValue;
            this.RowKey = !string.IsNullOrEmpty(id) ? id : throw new ArgumentNullException(nameof(id));

            this.Id = id;
            this.DisplayName = displayName;
            this.ProfileImageUrl = profileImageUrl;
            this.Gender = gender.ToString();
            this.Weight = weight;
        }
    }
}
