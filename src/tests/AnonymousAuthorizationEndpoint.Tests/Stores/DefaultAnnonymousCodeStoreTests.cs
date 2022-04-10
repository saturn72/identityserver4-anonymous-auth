using Dapper;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Stores;
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
using static IdentityServer4.Anonymous.Stores.DefaultAnonymousCodeStore;
using static IdentityServer4.Anonymous.Stores.SqlScripts;

namespace IdentityServer4.Anonymous.Tests.Stores
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
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            var res = await store.FindByUserCodeAsync(uc, false);
            res.ShouldBeNull();
        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindByUserCodeAsync_WithExpired_OrNot(bool i)
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var query = i ?
                AnonymousCodeScripts.SelectByUserCodeExcludeExpiredAndVerified :
                AnonymousCodeScripts.SelectByUserCode;

            var ci = new AnonymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnonymousCodeStore(() => db.Object, log.Object);
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
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            var res = await store.FindByVerificationCodeAsync(vc, false);
            res.ShouldBeNull();
        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FindByVerificationCodeAsync_WithExpired_OrNot(bool i)
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var query = i ?
                AnonymousCodeScripts.SelectByVeridicationCode :
                AnonymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;

            var ci = new AnonymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnonymousCodeStore(() => db.Object, log.Object);
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
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            var res = await store.FindByVerificationCodeAndUserCodeAsync(vc, uc);
            res.ShouldBeNull();
        }
        [Fact]
        public async Task FindByVerificationCodeAsync_ReturnsDBObject()
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var query = AnonymousCodeScripts.SelectByVerificationAndUserCodeExcludeExpiredAndVerified;

            var ci = new AnonymousCodeDbModel
            {
                Id = Guid.NewGuid(),
            };
            var db = new Mock<IDbConnection>();
            db.SetupDapperAsync(db => db.QueryAsync<AnonymousCodeDbModel>(It.Is<string>(s => s == query), null, null, null, null))
                .ReturnsAsync(new[] { ci });

            var store = new DefaultAnonymousCodeStore(() => db.Object, log.Object);
            var res = await store.FindByVerificationCodeAndUserCodeAsync("v-c", "u-c");
            res.Id.ShouldBe(ci.Id);
        }
        #endregion
        #region Store
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task StoreAnonymousCodeInfoAsync_Throws_On_MissingVerificaitonCode(string vc)
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            await Should.ThrowAsync<ArgumentException>(() => store.StoreAnonymousCodeInfoAsync(vc, null));
        }
        [Fact]
        public async Task StoreAnonymousCodeInfoAsync_Throws_On_NullCodeInfo()
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            await Should.ThrowAsync<ArgumentNullException>(() => store.StoreAnonymousCodeInfoAsync("v-c", default));
        }
        [Fact]
        public async Task StoreAnonymousCodeInfoAsync_Stores()
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();

            var ci = new AnonymousCodeDbModel
            {
                Id = Guid.NewGuid()
            };
            var db = new Mock<DbConnection>();
            db.SetupDapperAsync(d => d.ExecuteScalarAsync<AnonymousCodeDbModel>(
                It.Is<string>(s => s == AnonymousCodeScripts.InsertCommand),
                null, null, null, null))
              .ReturnsAsync(ci);
            var store = new DefaultAnonymousCodeStore(() => db.Object, log.Object);
            await store.StoreAnonymousCodeInfoAsync("v-c", new AnonymousCodeInfo()); ;
        }
        #endregion
        #region Update
        [Theory]
        [InlineData(default)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task UpdateVerificationRetryAsync_DoesNotThrow_OnMissingVerificaitonCode(string vc)
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();
            var store = new DefaultAnonymousCodeStore(null, log.Object);
            await store.UpdateVerificationRetryAsync(vc);
        }
        [Fact]
        public async Task UpdateVerificationRetryAsync_Updates()
        {
            var log = new Mock<ILogger<DefaultAnonymousCodeStore>>();

            var ci = new AnonymousCodeDbModel
            {
                Id = Guid.NewGuid()
            };
            var db = new Mock<DbConnection>();
            db.SetupDapperAsync(d => d.ExecuteScalarAsync<AnonymousCodeDbModel>(It.IsAny<string>(), null, null, null, null));
            var store = new DefaultAnonymousCodeStore(() => db.Object, log.Object);
            await store.UpdateVerificationRetryAsync("v-c");
        }
        #endregion
    }
}
