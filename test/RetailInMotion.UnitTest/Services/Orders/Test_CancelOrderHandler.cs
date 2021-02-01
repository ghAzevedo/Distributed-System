using Moq;
using RetailInMotion.Model.Dto;
using RetailInMotion.Model.Entities;
using RetailInMotion.Services.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.UnitTest.Services.Orders
{
    public class Test_CancelOrderHandler
    {
        private Mock<ICancelOrderHandlerDependencies> Dependencies;

        private void MockSetUp(Guid orderId, Guid productId)
        {
            Dependencies = new Mock<ICancelOrderHandlerDependencies>();
            Dependencies
                .Setup(a => a.GetOrderProductsByOrderId(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<OrderProductEntity>() { new OrderProductEntity(orderId, productId, 3) }.AsEnumerable()));
            Dependencies
                .Setup(a => a.GetOrderById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new OrderEntity(orderId, false, DateTime.UtcNow)));
            Dependencies
                .Setup(a => a.CancelOrderById(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            Dependencies
                .Setup(a => a.CancelDeliveryById(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            Dependencies
                .Setup(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()))
                .Returns(Task.CompletedTask);
            Dependencies.Setup(a => a.Commit())
                .Returns(Task.CompletedTask);
        }

        private void VerifyDependencies()
        {
            Dependencies.Verify(a => a.GetOrderProductsByOrderId(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.GetOrderById(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.CancelOrderById(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.CancelDeliveryById(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()));
        }

        [Fact]
        public async Task Test_Cancel_Order_OrderIsCorrect_Returns_Order()
        {
            var productId = Guid.NewGuid();
            var orderid = Guid.NewGuid();

            MockSetUp(orderid, productId);
            var handler = new CancelOrderHandler(Dependencies.Object);
            var result = await handler.Handle(orderid);

            Assert.True(result.Success);
            VerifyDependencies();
        }
    }
}
