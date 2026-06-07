using FluentValidation;
using HandCraft.Ortak.Sonuc;
using Microsoft.AspNetCore.Http;

namespace HandCraft.Ortak.Filters
{
    // Hocanin IsubuValidationFilter<T> kaliginin karsiligi.
    // Minimal API / controller endpoint filtresi olarak FluentValidation calistirir.
    // Govdedeki ilk T tipindeki argumani bulup dogrular; hata varsa 400 + ServisSonuc doner.
    public class HandCraftValidationFilter<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public HandCraftValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var model = context.Arguments.OfType<T>().FirstOrDefault();
            if (model is null)
            {
                return Results.BadRequest(
                    ServisSonuc.Hata($"Beklenen govde bulunamadi: {typeof(T).Name}"));
            }

            var sonuc = await _validator.ValidateAsync(model);
            if (!sonuc.IsValid)
            {
                var mesajlar = sonuc.Errors.Select(e => e.ErrorMessage);
                return Results.BadRequest(ServisSonuc.Hata(mesajlar));
            }

            return await next(context);
        }
    }
}
