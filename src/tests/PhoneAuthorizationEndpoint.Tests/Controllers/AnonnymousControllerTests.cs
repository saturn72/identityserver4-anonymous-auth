using IdentityServer4.Anonnymous.Events;
using IdentityServer4.Anonnymous.Services;
using IdentityServer4.Anonnymous.Stores;
using IdentityServer4.Anonnymous.UI;
using IdentityServer4.Anonnymous.UI.Controllers;
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

namespace PhoneAuthorizationEndpoint.Tests.Controllers
{
    public class AnonnymousControllerTests
    {
        [Fact]
        public void ValidateAttributesRoute()
        {
            var atts = typeof(AnonnymousController).GetCustomAttributesData();
            var ra = atts.First(a => a.AttributeType == typeof(RouteAttribute));
            ra.ConstructorArguments.Count.ShouldBe(1);
            ra.ConstructorArguments[0].Value.ShouldBe("anonnymous");
            _ = atts.First(a => a.AttributeType == typeof(SecurityHeadersAttribute));
        }
        [Fact]
        public void Index_HasHttpGetMethod()
        {
            var m = typeof(AnonnymousController).GetMethod(nameof(AnonnymousController.Index));
            var ga = m.GetCustomAttributesData().First(a => a.AttributeType == typeof(HttpGetAttribute));
            ga.ConstructorArguments.Count.ShouldBe(0);

        }
        [Fact]
        public async Task Index_MissingVerificationCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonnymousController>>();
            var ctrl = new AnonnymousController(null, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("BadOrMissingDataError");
        }
        [Fact]
        public async Task Index_NoMatchingEntryForVerificationCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonnymousController>>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "verification_code", "vc" } });

            var cs = new Mock<IAnnonymousCodeStore>();
            var ctrl = new AnonnymousController(cs.Object, null, null, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("BadOrMissingDataError");
            cs.Verify(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.Is<bool>(b => !b)), Times.Once);
        }
        [Fact]
        public async Task Index_MissingUserCode_ReturnsView()
        {
            var l = new Mock<ILogger<AnonnymousController>>();
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(new Dictionary<string, StringValues> { { "verification_code", "vc" } });

            var entry = new AnonnymousCodeInfo();
            var cs = new Mock<IAnnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);

            var ctrl = new AnonnymousController(cs.Object, null, null, l.Object)
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
            var l = new Mock<ILogger<AnonnymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonnymousCodeInfo();
            var cs = new Mock<IAnnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);

            var ctrl = new AnonnymousController(cs.Object, null, null, l.Object)
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
            var l = new Mock<ILogger<AnonnymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonnymousCodeInfo();
            var cs = new Mock<IAnnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);
            cs.Setup(c => c.FindByVerificationCodeAndUserCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(entry);

            var ev = new Mock<IEventService>();
            var inter= new Mock<IAnonnymousFlowInteractionService>();
            var ctrl = new AnonnymousController(cs.Object, inter.Object, ev.Object, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Error");
            ev.Verify(e => e.RaiseAsync(It.IsAny<AnonnymousGrantFailedEvent>()), Times.Once);
        }
        [Fact]
        public async Task Index_ReturnsSuccess()
        {
            var l = new Mock<ILogger<AnonnymousController>>();
            var d = new Dictionary<string, StringValues> {
                { "verification_code", "vc" },
                { "user_code", "uc" },
            };
            var ctx = new DefaultHttpContext();
            ctx.Request.Query = new QueryCollection(d);

            var entry = new AnonnymousCodeInfo();
            var cs = new Mock<IAnnonymousCodeStore>();
            cs.Setup(c => c.FindByVerificationCodeAsync(It.IsAny<string>(), It.IsAny<bool>()))
                   .ReturnsAsync(entry);
            cs.Setup(c => c.FindByVerificationCodeAndUserCodeAsync(It.IsAny<string>(), It.IsAny<string>()))
                   .ReturnsAsync(entry);

            var ev = new Mock<IEventService>();
            var inter = new Mock<IAnonnymousFlowInteractionService>();
            
            inter.Setup(i => i.HandleRequestAsync(It.IsAny<AnonnymousCodeInfo>()))
                   .ReturnsAsync(new AnonnymousInteractionResult());
            var ctrl = new AnonnymousController(cs.Object, inter.Object, ev.Object, l.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = ctx
                }
            };
            var res = await ctrl.Index();
            res.ShouldBeOfType<ViewResult>().ViewName.ShouldBe("Success");
            ev.Verify(e => e.RaiseAsync(It.IsAny<AnonnymousGrantedEvent>()), Times.Once);
        }
    }
}
