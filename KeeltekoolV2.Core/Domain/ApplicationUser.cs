using KeelteKool2.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace KeeltekoolV2.Core.Domain
{
    public class ApplicationUser : IdentityUser
    {
        //public ClaimsIdentity UserCredential { get; set; } = null;
        public string Placeholder { get; set; }

        public string Name { get; set; }
        public RegisterStatus AccountStatus { get; set; }
    }
}
