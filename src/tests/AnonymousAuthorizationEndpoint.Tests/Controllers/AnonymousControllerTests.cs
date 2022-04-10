using IdentityServer4.Anonymous.Events;
using IdentityServer4.Anonymous.Services;
using IdentityServer4.Anonymous.Stores;
using IdentityServer4.Anonymous.UI;
using IdentityServer4.Anonymous.UI.Controllers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identityserver4.Anonymous.Tests.Controllers
{
    public class AnonymousControllerTests
    {
        [Fact]
        public void ValidateAttributesRoute()
        {
            var atts = typeof(AnonymousController).GetCustomAttributesData();
            var ra = atts.First(a => a.AttributeType == typeof(RouteAttribute));
            ra.ConstructorArguments.Count.ShouldBe(1);
            ra.ConstructorArguments[0].Value.ShouldBe("anonymous");
            _ = atts.First(a => a.AttributeType == typeof(SecurityHeadersAttribute));
        }
        [Fact]
        public void Index_HasHttpGetMethod()
        {
            var m = typeof(AnonymousController).GetMethod(nameof(AnonymousController.Index));
            var ga = m.GetCustomAttributesData().First(a => a.AttributeType == typeof(HttpGetAttribute));
            ga.ConstructorArguments.Count.ShouldBe(0);

        }
        [Fact]
        public async Task Index_MissingVerificationCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var ctrl = new AnonymousController(null, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Error");
        }
        [Fact]
        public async Task Index_NoMatchingEntryForVerificationCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "verification_code", "vc" } });

            var cs = new Mock<IAnonymousCodeStore>();
            var ctrl = new AnonymousController(cs.Object, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Error");
            cs.Verify(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.Is<bool>(b => !b)), Times.Once);
        }
        [Fact]
        public async Task Index_MissingUserCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "verification_code", "vc" } });

            var entry = new AnonymousCodeInfo();
            var cs = new Mock<IAnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);

            var ctrl = new AnonymousController(cs.Object, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("UserCodeCapture");
        }
        [Fact]
        public async Task Index_HasUserCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonymousCodeInfo();
            var cs = new Mock<IAnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);

            var ctrl = new AnonymousController(cs.Object, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Error");
            cs.Verify(c => c.FindByVerificationCodeAndUserCodeAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        [Fact]
        public async Task Index_ReturnsErrorFromInteraction_ReturnsView()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonymousCodeInfo();
            var cs = new Mock<IAnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);
            cs.Setup(c => c.FindByVerificationCodeAndUserCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(entry);

            var ev = new Mock<IEventService>();
            var inter= new Mock<IAnonymousFlowInteractionService>();
            var ctrl = new AnonymousController(cs.Object, inter.Object, ev.Object, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Error");
            ev.Verify(e => e.RaiseAsync(It.IsAny<AnonymousGrantFailedEvent>()), Times.Once);
        }
        [Fact]
        public async Task Index_ReturnsSuccess()
        {
            var l = new Mock<ILogger<AnonymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonymousCodeInfo();
            var cs = new Mock<IAnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);
            cs.Setup(c => c.FindByVerificationCodeAndUserCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(entry);

            var ev = new Mock<IEventService>();
            var inter = new Mock<IAnonymousFlowInteractionService>();
            
            inter.Setup(i => i.HandleRequestAsync(It.IsAny<AnonymousCodeInfo>()))
                   .ReturnsAsync(new AnonymousInteractionResult());
            var ctrl = new AnonymousController(cs.Object, inter.Object, ev.Object, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Success");
            ev.Verify(e => e.RaiseAsync(It.IsAny<AnonymousGrantedEvent>()), Times.Once);
        }
    }
}
