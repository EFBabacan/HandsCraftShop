using HandCraft.Siparis.Application.Features.Commands.CreateSiparis.Dtos;
using MediatR;

namespace HandCraft.Siparis.Application.Features.Commands.CreateSiparis
{
    // CQRS Command (Bolum 4 madde 12). Donus: olusan siparis Id'si.
    public class CreateSiparisCommand : IRequest<int>
    {
        public string UserId { get; set; } = string.Empty;
        public CreateSiparisRequestDto Siparis { get; set; } = new();
    }
}
