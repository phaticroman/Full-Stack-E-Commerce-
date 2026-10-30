using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class Review : IAggregateRoot<Guid>
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Guid CustomerId { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Review() { }

        public static Review Create(Guid productId, Guid customerId, int rating, string? comment)
        {
            ValidateRating(rating);

            if (productId == Guid.Empty) throw new DomainException("ProductId is required.");
            if (customerId == Guid.Empty) throw new DomainException("CustomerId is required.");

            return new Review
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                ProductId = productId,
                CustomerId = customerId,
                Rating = rating,
                Comment = comment?.Trim(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(int rating, string? comment)
        {
            ValidateRating(rating);
            Rating = rating;
            Comment = comment?.Trim();
        }

        private static void ValidateRating(int rating)
        {
            if (rating is < 1 or > 5)
                throw new DomainException("Rating must be between 1 and 5.");
        }
    }
}
