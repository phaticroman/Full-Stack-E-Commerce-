using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Contracts.Services
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyAsync(string? token, string expectedAction, CancellationToken cancellationToken = default);
    }
}
