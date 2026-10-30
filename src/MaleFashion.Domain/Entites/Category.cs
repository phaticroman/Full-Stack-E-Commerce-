using MaleFashion.Domain.Contracts;
using MaleFashion.Domain.Exceptions;
using MaleFashion.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entites
{
    public class Category : IAggregateRoot<Guid>
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string? Description { get; private set; }
        public string? ImageName { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int DisplayOrder { get; private set; }

        private Category() { }

        public static Category Create(string name, string slug, string? description, string? imageName, int displayOrder, bool isActive = true)
        {
            Validate(name, slug);

            return new Category
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                Name = name.Trim(),
                Slug = slug.Trim().ToLowerInvariant(),
                Description = description?.Trim(),
                ImageName = imageName,
                DisplayOrder = displayOrder,
                IsActive = isActive
            };
        }

        public void UpdateDetails(string name, string slug, string? description, int displayOrder, bool isActive)
        {
            Validate(name, slug);

            Name = name.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Description = description?.Trim();
            DisplayOrder = displayOrder;
            IsActive = isActive;
        }

        public void ChangeImage(string imageName) => ImageName = imageName;
        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        private static void Validate(string name, string slug)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name is required.");
            if (string.IsNullOrWhiteSpace(slug))
                throw new DomainException("Category slug is required.");
        }
    }
}
