using HandCraft.Siparis.Application.Features.Queries.GetSiparislerByUserId.Dtos;
using MediatR;

namespace HandCraft.Siparis.Application.Features.Queries.GetTumSiparisler
{
    // CQRS Query: tum siparisler (admin paneli icin).
    public class GetTumSiparislerQuery : IRequest<List<SiparisListDto>>
    {
    }
}
