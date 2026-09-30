using KeeltekoolV2.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeeltekoolV2.Data
{
    public class KeeltekoolV2Context : IdentityDbContext<ApplicationUser>
    {
        public KeeltekoolV2Context(DbContextOptions<KeeltekoolV2Context> options) : base(options)
        {
        }
            //set tables here
            public DbSet<LanguageCourse> LanguageCourses { get; set; }
        }
    }
