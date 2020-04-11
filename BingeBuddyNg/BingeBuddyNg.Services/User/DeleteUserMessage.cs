﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BingeBuddyNg.Services.User
{
    public class DeleteUserMessage
    {
        public string UserId { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(UserId)}={UserId}}}";
        }
    }
}
