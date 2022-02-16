using Moq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.Anonnymous.Transport
{
    public class TransportExtensionsTests
    {
        [Fact]
        public async Task Transport_SendToArray()
        {
            var t1 = new Mock<ITransporter>();
            t1.Setup(t => t.ShouldHandle).Returns(c => Task.FromResult(true));

            var t2 = new Mock<ITransporter>();
            t2.Setup(t => t.ShouldHandle).Returns(c => Task.FromResult(false));
            var ctx = new UserCodeTransportContext
            {
                Body = "body",
                Data = "data",
                Provider = "p",
                Transport = "t"
            };
            await TransporterExtensions.Transport(new[] { t1.Object, t2.Object }, ctx);
            t1.Verify(t => t.Transport(It.Is<UserCodeTransportContext>(c => c == ctx)), Times.Once);
            t2.Verify(t => t.Transport(It.IsAny<UserCodeTransportContext>()), Times.Never);
        }
    }
}
