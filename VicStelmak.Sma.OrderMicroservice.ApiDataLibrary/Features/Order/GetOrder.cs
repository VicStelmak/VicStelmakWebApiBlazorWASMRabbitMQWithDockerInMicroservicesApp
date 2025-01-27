﻿using MediatR;

namespace VicStelmak.Sma.OrderMicroservice.ApiDataLibrary.Features.Order
{
    public record GetOrderByIdQuery(int OrderId) : IRequest<GetOrderResponse>;

    internal class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, GetOrderResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderResponse> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetOrderByIdAsync(query.OrderId);

            if (order != null) return order.MapToGetOrderResponse();

            // Unfortunately I had to return null because I require it if I want OrderEndpointsConfigurator to give a 404 response instead of exception thrown by
            // OrderMapper because of an attempt to pass null object to OrderMapper. I know it's a bad design practice but that is what Mapster also do in such cases.
            else return null;
        }
    }

    public record GetOrderResponse(
        int Id,
        int Apartment,
        string Building,
        string City,
        DateTime CreatedAt,
        string CreatedBy,
        string OrderCode,
        string PostalCode,
        int QuantityOfProducts,
        string Status,
        string Street,
        decimal Total,
        DateTime? UpdatedAt,
        string? UpdatedBy);
}
