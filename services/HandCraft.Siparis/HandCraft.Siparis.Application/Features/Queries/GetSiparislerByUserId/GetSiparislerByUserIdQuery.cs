using HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId.Dtos;
using MediatR;

namespace HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId
{
    // CQRS Query (Bolum 4 madde 12): bir kullanicinin tum siparisleri.
    public class GetSiparislerByUserIdQuery : IRequest<List<SiparisListDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
