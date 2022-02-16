using Dapper;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Stores;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Dapper;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static IdentityServer4.Anonnymous.Stores.DefaultAnnonymousCodeStore;
using static IdentityServer4.Anonnymous.Stores.SqlScripts;

namespace IdentityServer4.Anonnymous.Tests.Stores
{
    public class DefaultAnnonymousCodeStoreTests
    {
        #region FindByUserCodeAsync
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task FindByUserCodeAsync_Null_OnNullUserCode(string uc)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            var res = await store.FindByUserCodeAsync(uc, false);
            res.ShouldBeNull();
        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindByUserCodeAsync_WithExpired_OrNot(bool i)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var query = i ?
                AnonnymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified :
                AnonnymousCodeScripts.SelectByUserCode;

            var ci = new AnonnymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonnymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnnonymousCodeStore(() => db.Object, log.Object);
            var res = await store.FindByUserCodeAsync("u-c", i);
            res.Id.ShouldBe(ci.Id);
        }
        #endregion
        #region FindByVerificationCodeAsync
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task FindByVerificationCodeAsync_Null_OnNullVerificaitonCode(string vc)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            var res = await store.FindByVerificationCodeAsync(vc, false);
            res.ShouldBeNull();
        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindByVerificationCodeAsync_WithExpired_OrNot(bool i)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var query = i ?
                AnonnymousCodeScripts.SelectByVeridicationCode :
                AnonnymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;

            var ci = new AnonnymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonnymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnnonymousCodeStore(() => db.Object, log.Object);
            var res = await store.FindByVerificationCodeAsync("u-c", i);
            res.Id.ShouldBe(ci.Id);
        }
        #endregion
        #region FindByVerificationCodeAndUserCodeAsync
        [Theory]
        [InlineData(default, "uc")]
        [InlineData("", "uc")]
        [InlineData(" ", "uc")]
        [InlineData("vc", default)]
        [InlineData("vc", "")]
        [InlineData("vc", " ")]
        public async Task FindByVerificationCodeAndUserCodeAsync_Null_OnNullUserCode(string vc, string uc)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            var res = await store.FindByVerificationCodeAndUserCodeAsync(vc, uc);
            res.ShouldBeNull();
        }
        [Fact]
        public async Task FindByVerificationCodeAsync_ReturnsDBObject()
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var query = AnonnymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;

            var ci = new AnonnymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonnymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnnonymousCodeStore(() => db.Object, log.Object);
            var res = await store.FindByVerificationCodeAndUserCodeAsync("v-c", "u-c");
            res.Id.ShouldBe(ci.Id);
        }
        #endregion
        #region Store
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task StoreAnonnymousCodeInfoAsync_Throws_On_MissingVerificaitonCode(string vc)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            await Should.ThrowAsync<ArgumentException>(() => store.StoreAnonnymousCodeInfoAsync(vc, null));
        }
        [Fact]
        public async Task StoreAnonnymousCodeInfoAsync_Throws_On_NullCodeInfo()
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            await Should.ThrowAsync<ArgumentNullException>(() => store.StoreAnonnymousCodeInfoAsync("v-c", default));
        }
        [Fact]
        public async Task StoreAnonnymousCodeInfoAsync_Stores()
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();

            var ci = new AnonnymousCodeDbModel
            {
                Id = Guid.NewGuid()
            };
            var db = new Mock<DbConnection>();
            db.SetupDapperAsync(d => d.ExecuteScalarAsync<AnonnymousCodeDbModel>(
                It.Is<string>(s => s == AnonnymousCodeScripts.InsertCommand),
                null, null, null, null))
              .ReturnsAsync(ci);
            var store = new DefaultAnnonymousCodeStore(() => db.Object, log.Object);
            await store.StoreAnonnymousCodeInfoAsync("v-c", new AnonnymousCodeInfo()); ;
        }
        #endregion
        #region Update
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateVerificationRetryAsync_DoesNotThrow_OnMissingVerificaitonCode(string vc)
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();
            var store = new DefaultAnnonymousCodeStore(null, log.Object);
            await store.UpdateVerificationRetryAsync(vc);
        }
        [Fact]
        public async Task UpdateVerificationRetryAsync_Updates()
        {
            var log = new Mock<ILogger<DefaultAnnonymousCodeStore>>();

            var ci = new AnonnymousCodeDbModel
            {
                Id = Guid.NewGuid()
            };
            var db = new Mock<DbConnection>();
            db.SetupDapperAsync(d => d.ExecuteScalarAsync<AnonnymousCodeDbModel>(It.IsAny<string>(), null, null, null, null));
            var store = new DefaultAnnonymousCodeStore(() => db.Object, log.Object);
            await store.UpdateVerificationRetryAsync("v-c");
        }
        #endregion
    }
}
