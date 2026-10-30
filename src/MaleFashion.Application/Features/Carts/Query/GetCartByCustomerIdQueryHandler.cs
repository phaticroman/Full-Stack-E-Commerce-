using Cortex.Mediator.Queries;
using MaleFashion.Application.Contracts;
using MaleFashion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Carts.Query
{
    public class GetCartByCustomerIdQueryHandler : IQueryHandler<GetCartByCustomerIdQuery, CartDto>
    {
        private readonly IApplicationUnitOfwork _unitOfWork;

        public GetCartByCustomerIdQueryHandler(IApplicationUnitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> Handle(GetCartByCustomerIdQuery query, CancellationToken cancellationToken)
        {

            var cart = await _unitOfWork.CartRepository.GetByCustomerIdWithItemsAsync(query.CustomerId, cancellationToken);

            if (cart == null)
            {
                return new CartDto
                {
                    CustomerId = query.CustomerId,
                    IsEmpty = true,
                    TotalItems = 0,
                    SubTotal = 0m,
                    Items = new List<CartItemDto>()
                };
            }

            return cart.ToDto();
        }
    }
}
