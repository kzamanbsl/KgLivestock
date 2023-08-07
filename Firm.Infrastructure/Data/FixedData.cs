﻿using Firm.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firm.Infrastructure.Data
{
    public class FixedData
    {
        public static void SeedData(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "Hotel", NormalizedName = "HOTEL" }
           );
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "0f04028e-587c-47ad-8b36-6dbd6a059fa4",
                    PhoneNumber = "01700729807",
                    Email = "tclagro@gmail.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    NormalizedEmail = "tclagro@gmail.com",
                    NormalizedUserName = "TCL",
                    //PasswordHash = "AQAAAAEAACcQAAAAEE8d8uAFK+zBNJ3j+s3k5c6D+OqrJJqgpV0CF42z2UDwqm/kSD/LWNXN8OAx/56YHg==",
                    PasswordHash = "AQAAAAEAACcQAAAAEBT4bq172HFgnbZSRzxXcBg37XdddVWlPRLNH472YWlF2fzFbzmLwDaOmBDL+K/M4g==",
                    ConcurrencyStamp = "616a2e8f-dc94-4576-8ec4-c9d75d1df6d1",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = "37QJAUUNCSSXNFFB6ZXI6OJLHSCS5J6I",
                    TwoFactorEnabled = false,
                    UserName = "tcl",
                    PreFix = "tcl123456",
                    Name = "System Admin",
                    UserType = 1,
                    IsActive = true,
                },
                new ApplicationUser
                {
                    Id = "0f04028e-587c-48ad-8b36-6dbd6a059fa5",
                    PhoneNumber = "01700729808",
                    Email = "dil.afroza@krishibidgroup.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    NormalizedEmail = "dil.afroza@krishibidgroup.com",
                    NormalizedUserName = "HOTEL",
                    //PasswordHash = "AQAAAAEAACcQAAAAEE8d8uAFK+zBNJ3j+s3k5c6D+OqrJJqgpV0CF42z2UDwqm/kSD/LWNXN8OAx/56YHg==",
                    PasswordHash = "AQAAAAEAACcQAAAAEBT4bq172HFgnbZSRzxXcBg37XdddVWlPRLNH472YWlF2fzFbzmLwDaOmBDL+K/M4g==",
                    ConcurrencyStamp = "616a2e8f-dc94-4576-8ec4-c9d75d1df6d2",
                    PhoneNumberConfirmed = true,
                    SecurityStamp = "37QJAUUNCSSXNFFB6ZXI6OJLHSCS5J62",
                    TwoFactorEnabled = false,
                    UserName = "Hotel",
                    PreFix = "Ji123456",
                    UserType = 3,
                    IsActive = true,
                }
            );

            builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                UserId = "0f04028e-587c-47ad-8b36-6dbd6a059fa4"
            },
             new IdentityUserRole<string>
             {
                 RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                 UserId = "0f04028e-587c-48ad-8b36-6dbd6a059fa5"
             }
         );
        }
    }
}


