using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Susanoo.Pipeline.Command;

namespace Susanoo.Tests
{
    [TestFixture]
    public class MT2
    {
        [Test]
        public void Test()
        {
            var LoadUserBaseCommand =
                CommandManager.DefineCommand(@"SELECT TOP 1 * FROM dbo.AuthUser WHERE AuthUserId = @userId",
                    CommandType.Text)
                    .IncludeProperty("userId");

            ICommandProcessor<dynamic, dynamic, UserModel> FindUsersCommand =
                CommandManager.DefineCommand(@"SELECT TOP 1 [Count] = COUNT(*)
                            FROM dbo.AuthUser au
                            WHERE au.AuthUserTypeId = @Internal AND au.DisplayName LIKE @NameQuery 
                                AND (@Deleted IS NULL OR au.IsDeleted = @Deleted)

                            SELECT au.*, d.DepartmentName
                            FROM dbo.AuthUser au
                            LEFT JOIN dbo.OrgDepartment d ON d.OrgDepartmentId = au.DepartmentId
                            WHERE au.AuthUserTypeId = @Internal AND au.DisplayName LIKE @NameQuery 
                                AND (@Deleted IS NULL OR au.IsDeleted = @Deleted)
                            ORDER BY AuthUserId
                            OFFSET @Skip ROWS
                            FETCH NEXT @Take ROWS ONLY", CommandType.Text)
                    .UseExplicitPropertyInclusionMode()
                    .SendNullValues()
                    .AddConstantParameter("Internal", parameter => parameter.Value = 1)
                    .IncludeProperty("NameQuery")
                    .IncludeProperty("Deleted")
                    .IncludeProperty("Skip")
                    .IncludeProperty("Take")
                    .DefineResults<dynamic, UserModel>()
                    .Realize("SearchUsers");

            ICommandProcessor<dynamic, UserModel> LoadUserCommand =
                LoadUserBaseCommand
                    .DefineResults<UserModel>()
                    .Realize("LoadUser");
        }
    }

    public class UserModel
    {
        public int AuthUserId { get; set; }
        public int AuthUserTypeId { get; set; }
        public string LDAPDomain { get; set; }
        public string LDAPUserLogin { get; set; }

        [DisplayName("Name")]
        public string DisplayName { get; set; }

        public DateTime LastLoginDate { get; set; }

        [DisplayName("Extension")]
        public string DeskPhoneExt { get; set; }

        [DisplayName("Department")]
        public int DepartmentId { get; set; }
        [DisplayName("Department")]
        public string DepartmentName { get; set; }

        public int? ManagerAuthUserId { get; set; }

        public string ManagerAuthUserName { get; set; }

        [DisplayName("Currently Out of Office")]
        public bool IsOutOfOffice { get; set; }

        public bool IsDeleted { get; set; }

        public string EmailAddress { get; set; }

        public string AdminEmailAddress { get; set; }

        public bool HasEmployees { get; set; }

        [DisplayName("In Training")]
        public bool IsTraining { get; set; }
    }
}
