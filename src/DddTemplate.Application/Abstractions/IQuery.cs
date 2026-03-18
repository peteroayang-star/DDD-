using MediatR;

namespace DddTemplate.Application.Abstractions;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
