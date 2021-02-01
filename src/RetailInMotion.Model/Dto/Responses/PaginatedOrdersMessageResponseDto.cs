using System.Collections.Generic;

namespace RetailInMotion.Model.Dto
{
    public class PaginatedOrdersMessageResponseDto
    {
        public List<OrderDto> Orders { get; set; }
    }
}