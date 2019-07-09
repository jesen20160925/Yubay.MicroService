using AspNetCoreMvcAuthSample.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMvcAuthSample.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationUserRole,int>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

        //public DbSet<ApplicationUser> Users { get; set; }

        //public DbSet<ApplicationUserRole> UserRoles { get; set; }
	}
}
