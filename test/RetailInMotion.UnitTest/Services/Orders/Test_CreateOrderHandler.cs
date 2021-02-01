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
    public class Test_CreateOrderHandler
    {
        private Mock<ICreateOrderHandlerDependencies> Dependencies;

        private void MockSetUp(Guid orderId, Guid productId)
        {
            Dependencies = new Mock<ICreateOrderHandlerDependencies>();
            Dependencies
                .Setup(a => a.GetProductByIds(It.IsAny<List<Guid>>()))
                .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(productId, "name", 20) }.AsEnumerable()));
            Dependencies
                .Setup(a => a.AddDelivery(It.IsAny<DeliveryEntity>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            Dependencies
                .Setup(a => a.AddOrderProducts(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);
            Dependencies
                .Setup(a => a.AddOrder())
                .Returns(Task.FromResult(orderId));
            Dependencies
                .Setup(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()))
                .Returns(Task.CompletedTask);
            Dependencies.Setup(a => a.Commit())
                .Returns(Task.CompletedTask);
        }

        private void VerifyDependencies()
        {
            Dependencies.Verify(a => a.GetProductByIds(It.IsAny<List<Guid>>()));
            Dependencies.Verify(a => a.AddDelivery(It.IsAny<DeliveryEntity>(), It.IsAny<Guid>()));
            Dependencies.Verify(a => a.AddOrderProducts(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()));
            Dependencies.Verify(a => a.AddOrder());
            Dependencies.Verify(a => a.UpdateProductStock(It.IsAny<List<(Guid, int)>>()));
            Dependencies.Verify(a => a.Commit());
        }

        public OrderCreationDto CreateDefaultOrder(Guid productGuid)
        {
            List<ProductQuantityDto> productQties = new List<ProductQuantityDto>()
                    {
                        new ProductQuantityDto(){ ProductId = productGuid, Quantity = 20 }
                    };
            DeliveryDto delivery = new DeliveryDto("country", "city", "street");

            return new OrderCreationDto() { Products = productQties, Delivery = delivery };
        }

        [Fact]
        public async Task Test_Create_Order_OrderIsCorrect_Returns_OrderId()
        {
            var productId = Guid.NewGuid();
            var defaultOrder = CreateDefaultOrder(productId);
            var orderid = Guid.NewGuid();

            MockSetUp(orderid, productId);
            var handler = new CreateOrderHandler(Dependencies.Object);
            var result = await handler.Handle(defaultOrder);

            Assert.True(result.Success);
            Assert.Equal(orderid, result.Value.OrderId);
            VerifyDependencies();
        }

        [Fact]
        public async Task Test_Create_Order_ProductIsInvalid_ReturnsFailure()
        {
            var productId = Guid.NewGuid();
            var defaultOrder = CreateDefaultOrder(productId);
            var orderid = Guid.NewGuid();

            MockSetUp(orderid, productId);

            Dependencies
                .Setup(a => a.GetProductByIds(It.IsAny<List<Guid>>()))
                .Returns(Task.FromResult(new List<ProductEntity>().AsEnumerable()));

            var handler = new CreateOrderHandler(Dependencies.Object);
            var result = await handler.Handle(defaultOrder);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }


        [Fact]
        public async Task Test_Creation_Order_OrderStockIsIncorrect_ReturnsFailure()
        {
            var productId = Guid.NewGuid();
            var defaultOrder = CreateDefaultOrder(productId);
            var orderid = Guid.NewGuid();

            MockSetUp(orderid, productId);

            Dependencies
                .Setup(a => a.GetProductByIds(It.IsAny<List<Guid>>()))
                .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(productId, "name", 1) }
                .AsEnumerable()));

            var handler = new CreateOrderHandler(Dependencies.Object);
            var result = await handler.Handle(defaultOrder);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }
    }
}
