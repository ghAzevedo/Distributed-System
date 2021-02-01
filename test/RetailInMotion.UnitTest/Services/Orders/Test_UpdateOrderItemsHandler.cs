using Moq;
using RetailInMotion.Model;
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
    public class Test_UpdateOrderItemsHandler
    {
        private Mock<IUpdateOrderItemsHandlerDependencies> Dependencies;

        private void MockSetUp(Guid orderId, Guid productId)
        {
            Dependencies = new Mock<IUpdateOrderItemsHandlerDependencies>();
            Dependencies
                .Setup(a => a.GetOrderProductsByOrderId(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new List<OrderProductEntity>() { new OrderProductEntity(orderId, productId, 20)}.AsEnumerable()));
            Dependencies.Setup(a => a.GetProductById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new ProductEntity(productId, "name", 20)));
            Dependencies.Setup(a => a.GetOrderById(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new OrderEntity(orderId, false, DateTime.UtcNow)));
            Dependencies.Setup(a => a.UpdateOrderProductQuantity(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            Dependencies.Setup(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()))
                .Returns(Task.CompletedTask);
            Dependencies.Setup(a => a.Commit())
                .Returns(Task.CompletedTask);
        }

        private void VerifyDependencies()
        {
            Dependencies.Verify(a => a.GetOrderProductsByOrderId(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.GetProductById(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.GetOrderById(It.IsAny<Guid>()));
            Dependencies.Verify(a => a.UpdateOrderProductQuantity(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()));
            Dependencies.Verify(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()));
        }

        [Fact]
        public async Task Test_Update_OrderItem_StockIsCorrect_Returns_Sucess()
        {
            var productId = Guid.NewGuid();
            var orderid = Guid.NewGuid();

            var orderItems = new OrderItems
            {
                OrderId = orderid,
                ProductsQty = new List<ProductQuantityDto> { new ProductQuantityDto { ProductId = productId, Quantity = 15 } }
            };

            MockSetUp(orderid, productId);
            var handler = new UpdateOrderItemsHandler(Dependencies.Object);
            var result = await handler.Handle(orderItems);

            Assert.True(result.Success);
            VerifyDependencies();
        }
    }
}
