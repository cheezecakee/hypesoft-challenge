using Hypesoft.Application.Commands.Products;
using Hypesoft.Domain.Repositories;
using MediatR;

namespace Hypesoft.Application.Handlers.Products
{
    public class DeleteProductCommandHandler(
            IProductRepository productRepository
            ) : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            bool productExists = await _productRepository.ExistsAsync(request.Id, cancellationToken);
            if (!productExists)
            {
                return false;
            }

            await _productRepository.DeleteAsync(request.Id, cancellationToken);
            return true;
        }
    }
}
